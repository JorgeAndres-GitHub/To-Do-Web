using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using ToDo_Backend_CA_AplicationLayer.UseCases.TokenUseCases;
using ToDo_Backend_CA_AplicationLayer.UseCases.UserUseCase;
using ToDo_Backend_CA_AplicationLayer.UseCases.UserUseCases;
using ToDo_Backend_CA_EnterpriseLayer;
using ToDo_Backend_CA_InterfaceAdapters_Data;
using ToDo_Backend_FrameworksDrivers_API.Configuration;
using ToDo_Backend_FrameworksDrivers_API.Services;
using ToDo_Backend_InterfaceAdapters_Mappers.Auth;
using ToDo_Backend_InterfaceAdapters_Mappers.DTOs;
using ToDo_Backend_InterfaceAdapters_Mappers.DTOs.Requests.User;
using ToDo_Backend_InterfaceAdapters_Mappers.DTOs.Requests.UserRequests;
using ToDo_Backend_InterfaceAdapters_Models;

namespace ToDo_Backend_FrameworksDrivers_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly RegisterUseCase<UserRegistrationRequestDto, AuthResult> _registerUseCase;
        private readonly LoginUseCase<AuthResult> _loginUseCase;
        private readonly AddRefreshTokenUseCase<RefreshTokenModel, AuthResult> _addRefreshTokenUseCase;
        private readonly GetRefreshTokenUseCase<RefreshTokenModel, AuthResult> _getRefreshTokenUseCase;
        private readonly UpdateRefreshTokenUseCase<RefreshTokenModel, AuthResult> _updateRefreshTokenUseCase;
        private readonly JwtConfig _jwtConfig;
        private readonly IEmailSender _emailSender;
        private readonly AppDbContext _context;
        private readonly TokenValidationParameters _tokenValidationParameters;

        public AuthenticationController(RegisterUseCase<UserRegistrationRequestDto, AuthResult> registerUseCase, LoginUseCase<AuthResult> loginUseCase,
            AddRefreshTokenUseCase<RefreshTokenModel, AuthResult> addRefreshTokenUseCase, GetRefreshTokenUseCase<RefreshTokenModel, AuthResult> getRefreshTokenUseCase, 
            UpdateRefreshTokenUseCase<RefreshTokenModel, AuthResult> updateRefreshTokenUseCase
            , IOptions<JwtConfig> jwtConfig, IEmailSender emailSender,
            AppDbContext context, TokenValidationParameters tokenValidationParameters)
        {
            _registerUseCase = registerUseCase;
            _loginUseCase = loginUseCase;
            _addRefreshTokenUseCase = addRefreshTokenUseCase;
            _getRefreshTokenUseCase = getRefreshTokenUseCase;
            _updateRefreshTokenUseCase = updateRefreshTokenUseCase;
            _jwtConfig = jwtConfig.Value;
            _emailSender = emailSender;
            _context = context;
            _tokenValidationParameters = tokenValidationParameters;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequestDto registerRequest)
        {
            var userAuthResult = await _registerUseCase.ExecuteAsync(registerRequest);

            await SendVerificationEmail(userAuthResult.User);

            return Ok(new AuthResult
            {
                Result = true
            });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDto loginRequest)
        {
            var success = await _loginUseCase.ExecuteAsync(loginRequest.Email, loginRequest.Password);
            if(!success.Result)
                return BadRequest(success);

            var tokenResult = await GenerateTokenService.GenerateTokenAsync(_addRefreshTokenUseCase, success.User, _jwtConfig);
            success.Token = tokenResult.Token;
            success.RefreshToken = tokenResult.RefreshToken;

            return Ok(success);
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequestDto tokenRequestDto)
        {
            var userId = await VerifyAndGenerateTokenService.VerifyAndGenerateTokenAsync(tokenRequestDto, _tokenValidationParameters, _getRefreshTokenUseCase, _updateRefreshTokenUseCase);
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return BadRequest(new AuthResult
                {
                    Errors = new List<string> { "Invalid token" }
                });

            var result = await GenerateTokenService.GenerateTokenAsync(_addRefreshTokenUseCase, user, _jwtConfig);               

            return Ok(result);
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
                return BadRequest(new AuthResult
                {
                    Result = false,
                    Errors = new List<string> { "Invalid email confirmation url" }
                }
                );

            var user = await _context.Users.FindAsync(int.Parse(userId));

            if (user == null)
                return NotFound($"Unable to load user with ID '{userId}'.");

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

            user.IsEmailConfirmed = true;
            await _context.SaveChangesAsync();

            return Ok("Thank you for confirming your email.");
        }


        private async Task SendVerificationEmail(UserModel user)
        {
            var verificationCode = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            verificationCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(verificationCode));

            // example : https://localhost:8080/authentication/verifyEmail/userId=exampleuserId&code=exampleCode
            var callbackUrl = $"{Request.Scheme}://{Request.Host}{Url.Action("ConfirmEmail", controller: "Authentication", new { UserId = user.Id, code = verificationCode })}";

            var emailBody = $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.";

            await _emailSender.SendEmailAsync(user.Email, "Confirm your email", emailBody);

        }
    }
}

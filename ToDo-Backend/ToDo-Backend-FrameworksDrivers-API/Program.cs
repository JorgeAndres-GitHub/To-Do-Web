using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using NLog;
using NLog.Web;
using Scalar.AspNetCore;
using System.Text;
using ToDo_Backend_CA_AplicationLayer.Interfaces;
using ToDo_Backend_CA_AplicationLayer.Interfaces.TaskAplicationInterfaces;
using ToDo_Backend_CA_AplicationLayer.Interfaces.User;
using ToDo_Backend_CA_AplicationLayer.Interfaces.UserAplicationInterfaces;
using ToDo_Backend_CA_AplicationLayer.UseCases.TaskUseCases;
using ToDo_Backend_CA_AplicationLayer.UseCases.TokenUseCases;
using ToDo_Backend_CA_AplicationLayer.UseCases.UserUseCase;
using ToDo_Backend_CA_AplicationLayer.UseCases.UserUseCases;
using ToDo_Backend_CA_EnterpriseLayer;
using ToDo_Backend_CA_IntefaceAdapters_Presenters.Presenters;
using ToDo_Backend_CA_IntefaceAdapters_Presenters.Views;
using ToDo_Backend_CA_InterfaceAdapters_Data;
using ToDo_Backend_FrameworksDrivers_API.Configuration;
using ToDo_Backend_FrameworksDrivers_API.Middlewares;
using ToDo_Backend_FrameworksDrivers_API.Services;
using ToDo_Backend_FrameworksDrivers_API.Validators.Task;
using ToDo_Backend_InterfaceAdapters_Mappers.Auth;
using ToDo_Backend_InterfaceAdapters_Mappers.DTOs.Requests.Task;
using ToDo_Backend_InterfaceAdapters_Mappers.DTOs.Requests.User;
using ToDo_Backend_InterfaceAdapters_Mappers.DTOs.Requests.UserRequests;
using ToDo_Backend_InterfaceAdapters_Mappers.Mappers.TaskMappers;
using ToDo_Backend_InterfaceAdapters_Mappers.Mappers.UserMappers;
using ToDo_Backend_InterfaceAdapters_Models;
using ToDo_Backend_InterfaceAdapters_Repository;


var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("Starting application");

try
{
    var builder = WebApplication.CreateBuilder(args);
    // Add services to the container.

    builder.Services.AddControllers().AddNewtonsoftJson();
    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi("v1", options => { options.AddDocumentTransformer<BearerSecuritySchemeTransformer>(); });

    // Validators
    builder.Services.AddValidatorsFromAssemblyContaining<AddTaskValidator>();
    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddFluentValidationClientsideAdapters();

    // CORS
    builder.Services.AddCors(policyBuilder => policyBuilder.AddDefaultPolicy(policy => policy.WithOrigins("*").AllowAnyHeader().AllowAnyMethod()));

    // Dependencies
    string connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<AppDbContext>(options =>
    {
        options.UseSqlServer(connectionString);
    });



    // Email
    builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
    builder.Services.AddSingleton<IEmailSender, EmailService>();

    // JWT
    builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));

    var key = Encoding.ASCII.GetBytes(builder.Configuration.GetSection("JwtConfig:Secret").Value ?? Environment.GetEnvironmentVariable("JwtConfig__Secret"));
    var tokenValidationsParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false, // En produccion tiene que ser verdadero
        ValidateAudience = false, // En produccion debe ser verdadero
        RequireExpirationTime = false, // Falso por ahora
        ValidateLifetime = true
    };

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(jwt =>
    {
        jwt.SaveToken = true;
        jwt.TokenValidationParameters = tokenValidationsParameters;
    });
    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("TaskPublisher", policy => policy.RequireClaim("Role", "1"));
    });


    builder.Services.AddSingleton(tokenValidationsParameters);

    //Task Dependencies
    builder.Services.AddScoped<ITaskRepository<TaskItem>, TaskRepository>();
    builder.Services.AddScoped<IMapper<TaskRequestDto, TaskItem>, TaskMapper>();
    builder.Services.AddScoped<IMapper<UpdateTaskRequestDto, TaskItem>, UpdateTaskMapper>();
    builder.Services.AddScoped<IPresenter<TaskItem, TaskViewModel>, TaskPresenter>();
    builder.Services.AddScoped<AddTaksUseCase<TaskRequestDto>>();
    builder.Services.AddScoped<GetTaskUseCase<TaskItem>>();
    builder.Services.AddScoped<GetAllTasksUseCase<TaskItem, TaskViewModel>>();
    builder.Services.AddScoped<DeleteTaskUseCase>();
    builder.Services.AddScoped<DeleteMultipleTasksUseCase>();
    builder.Services.AddScoped<UpdateTaskUseCase<UpdateTaskRequestDto>>();
    builder.Services.AddScoped<MarkAsCompletedUseCase>();
    builder.Services.AddScoped<GetAllUserTasksUseCase<TaskItem, TaskViewModel>>();
    builder.Services.AddScoped<PostTaskUseCase>();

    //User Dependencies
    builder.Services.AddScoped<IAccountRepository<UserEntity, AuthResult>, AccountRepository>();
    builder.Services.AddScoped<ITokenRepository<RefreshTokenModel, AuthResult>, TokenRepository>();
    builder.Services.AddScoped<IMapper<UserRegistrationRequestDto, UserEntity>, UserMapper>();
    builder.Services.AddScoped<IAccountPresenter<UserEntity, UserViewModel>, UserPresenter>();
    builder.Services.AddScoped<RegisterUseCase<UserRegistrationRequestDto, AuthResult>>();
    builder.Services.AddScoped<LoginUseCase<AuthResult>>();
    builder.Services.AddScoped<GetProfileViewModelUseCase<UserEntity, AuthResult, UserViewModel>>();
    builder.Services.AddScoped<GetProfileUseCase<UserEntity, AuthResult>>();
    builder.Services.AddScoped<UpdateProfileUseCase<AuthResult>>();
    builder.Services.AddScoped<DeleteProfileUseCase<AuthResult>>();
    builder.Services.AddScoped<AddRefreshTokenUseCase<RefreshTokenModel, AuthResult>>();
    builder.Services.AddScoped<GetRefreshTokenUseCase<RefreshTokenModel, AuthResult>>();
    builder.Services.AddScoped<UpdateRefreshTokenUseCase<RefreshTokenModel, AuthResult>>();

    // NLog
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference(options =>
        {
            options
                .WithTitle("To Do API")
                .WithTheme(ScalarTheme.Mars)
                .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
        });
    }

    app.UseCors();

    app.UseHttpsRedirection();

    app.UseMiddleware<ExceptionHandlingMiddleware>();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception e)
{
    logger.Error(e, "There has been an error");
    throw;
}
finally
{
    NLog.LogManager.Shutdown(); 
}
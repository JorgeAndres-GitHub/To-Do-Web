using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;
using ToDo_Backend_CA_AplicationLayer.Interfaces;
using ToDo_Backend_CA_AplicationLayer.Interfaces.TaskAplicationInterfaces;
using ToDo_Backend_CA_AplicationLayer.Interfaces.User;
using ToDo_Backend_CA_AplicationLayer.Interfaces.UserAplicationInterfaces;
using ToDo_Backend_CA_AplicationLayer.UseCases.TaskUseCases;
using ToDo_Backend_CA_AplicationLayer.UseCases.UserUseCase;
using ToDo_Backend_CA_AplicationLayer.UseCases.UserUseCases;
using ToDo_Backend_CA_EnterpriseLayer;
using ToDo_Backend_CA_IntefaceAdapters_Presenters.Presenters;
using ToDo_Backend_CA_IntefaceAdapters_Presenters.Views;
using ToDo_Backend_CA_InterfaceAdapters_Data;
using ToDo_Backend_FrameworksDrivers_API.Configuration;
using ToDo_Backend_FrameworksDrivers_API.Middlewares;
using ToDo_Backend_FrameworksDrivers_API.Validators.Task;
using ToDo_Backend_InterfaceAdapters_Mappers.Auth;
using ToDo_Backend_InterfaceAdapters_Mappers.DTOs.Requests.Task;
using ToDo_Backend_InterfaceAdapters_Mappers.DTOs.Requests.User;
using ToDo_Backend_InterfaceAdapters_Mappers.Mappers;
using ToDo_Backend_InterfaceAdapters_Mappers.Mappers.UserMappers;
using ToDo_Backend_InterfaceAdapters_Models;
using ToDo_Backend_InterfaceAdapters_Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi("v1", options => { options.AddDocumentTransformer<BearerSecuritySchemeTransformer>(); });

// Validators
builder.Services.AddValidatorsFromAssemblyContaining<AddTaskValidator>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

// CORS
builder.Services.AddCors(policyBuilder => policyBuilder.AddDefaultPolicy(policy => policy.WithOrigins("*").AllowAnyHeader().AllowAnyMethod()));

// Dependencies
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwt =>
{
    var key = Encoding.ASCII.GetBytes(builder.Configuration.GetSection("JwtConfig:Secret").Value);

    jwt.SaveToken = true;
    jwt.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false, // En produccion tiene que ser verdadero
        ValidateAudience = false, // En produccion debe ser verdadero
        RequireExpirationTime = false, // Falso por ahora
        ValidateLifetime = true
    };
});


builder.Services.AddScoped<ITaskRepository<TaskItem>, TaskRepository>();
builder.Services.AddScoped<IMapper<TaskRequestDTO, TaskItem>, TaskMapper>();
builder.Services.AddScoped<IMapper<UpdateTaskRequestDTO, TaskItem>,  UpdateTaskMapper>();
builder.Services.AddScoped<IPresenter<TaskItem, TaskViewModel>, TaskPresenter>();
builder.Services.AddScoped<AddTaksUseCase<TaskRequestDTO>>();
builder.Services.AddScoped<GetTaskUseCase<TaskItem>>();
builder.Services.AddScoped<GetAllTasksUseCase<TaskItem, TaskViewModel>>();
builder.Services.AddScoped<DeleteTaskUseCase>();
builder.Services.AddScoped<DeleteMultipleTasksUseCase>();
builder.Services.AddScoped<UpdateTaskUseCase<UpdateTaskRequestDTO>>();
builder.Services.AddScoped<MarkAsCompletedUseCase>();
builder.Services.AddScoped<GetAllUserTasksUseCase<TaskItem, TaskViewModel>>();

builder.Services.AddScoped<IAccountRepository<UserEntity, AuthResult>, AccountRepository>();
builder.Services.AddScoped<IMapper<UserRegistrationRequestDTO, UserEntity>, UserMapper>();
builder.Services.AddScoped<IAccountPresenter<UserEntity, UserViewModel>, UserPresenter>();
builder.Services.AddScoped<RegisterUseCase<UserRegistrationRequestDTO, AuthResult>>();
builder.Services.AddScoped<LoginUseCase<AuthResult>>();
builder.Services.AddScoped<GetProfileUseCase<UserEntity, AuthResult, UserViewModel>>();




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
using AutoMapper;
using ChatService.AppSettingsOptions;
using ChatService.AsyncDataServices;
using ChatService.Common.Constants;
using ChatService.Common.Exceptions;
using ChatService.Common.MappingProfile;
using ChatService.Data;
using ChatService.Data.Repositories.Implementations;
using ChatService.Data.Repositories.Interfaces;
using ChatService.EventProcessing.Implementations;
using ChatService.EventProcessing.Interfaces;
using ChatService.Services.Implementations;
using ChatService.Services.Interfaces;
using ChatService.SyncDataServices.Grpc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var securityOptions = new SecurityOptions();
builder.Configuration.Bind(nameof(SecurityOptions), securityOptions);
builder.Services.AddSingleton(securityOptions);

var rabbitMQOptions = new RabbitMQOptions();
builder.Configuration.Bind(nameof(RabbitMQOptions), rabbitMQOptions);
builder.Services.AddSingleton(rabbitMQOptions);

builder.Services.AddDbContext<ChatContext>(opt => opt.UseSqlServer(
    builder.Configuration.GetConnectionString(AppConstants.ConnectionStringName)
));

// REPOSITORIES
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IChatRepository, ChatRepository>();
builder.Services.AddTransient<IMessageRepository, MessageRepository>();

// SERVICES
// api services
builder.Services.AddTransient<IChatService, ChatService.Services.Implementations.ChatService>();


// events services
builder.Services.AddTransient<IEventDeterminator, EventDeterminator>();
builder.Services.AddTransient<IEventProcessor, EventProcessor>();
builder.Services.AddHostedService<MessageBusSubscriber>();

// grpc services
builder.Services.AddScoped<IAuthenticationDataClient, AuthenticationDataClient>();

// general purposes
builder.Services.AddTransient<IEventsService, EventsService>();
builder.Services.AddTransient<IFilesService, FilesService>();

builder.Services.AddSingleton(
    new MapperConfiguration(mc =>
        mc.AddProfile(new ChatMappingProfile()))
    .CreateMapper());


builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Scheme = "Bearer",

    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme,
                }
            },
            new string[]{}
        }
    });
});

builder.Services
   .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(cfg =>
    {
        var rsa = RSA.Create();
        var key = File.ReadAllText(securityOptions.PublicKeyFilePath);
        rsa.FromXmlString(key);

        cfg.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = securityOptions.Issuer,
            ValidAudience = securityOptions.Audience,

            IssuerSigningKey = new RsaSecurityKey(rsa),
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(a => a.Run(async context =>
{
    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
    var exception = exceptionHandlerPathFeature?.Error;

    if (exception is CustomException)
    {
        context.Response.StatusCode = (exception as CustomException)!.StatusCode;
    }

    await context.Response.WriteAsJsonAsync(new { error = exception!.Message });
}));
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await DbPreparator.PrepareDb(app, app.Configuration);

app.Run();

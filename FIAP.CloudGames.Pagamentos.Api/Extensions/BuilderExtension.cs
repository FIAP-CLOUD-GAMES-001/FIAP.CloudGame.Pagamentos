using FIAP.CloudGames.Pagamentos.Api.Filters;
using FIAP.CloudGames.Pagamentos.Api.Logging;
using FIAP.CloudGames.Pagamentos.Domain.Interfaces.Repositoiries;
using FIAP.CloudGames.Pagamentos.Domain.Interfaces.Services;
using FIAP.CloudGames.Pagamentos.Domain.Models;
using FIAP.CloudGames.Pagamentos.Infrastructure.Data;
using FIAP.CloudGames.Pagamentos.Infrastructure.Health;
using FIAP.CloudGames.Pagamentos.Infrastructure.Repositories;
using FIAP.CloudGames.Pagamentos.Service.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Serilog;
using Serilog.Events;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace FIAP.CloudGames.Pagamentos.Api.Extensions
{
    public static class BuilderExtension
    {
        public static void AddProjectServices(this WebApplicationBuilder builder)
        {
            builder.UseJsonFileConfiguration();
            builder.ConfigureMongoDbContext();
            builder.ConfigureJwt();
            builder.ConfigureLogMongo();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.ConfigureSwagger();
            builder.ConfigureDependencyInjectionRepository();
            builder.ConfigureDependencyInjectionService();
            builder.ConfigureHealthChecks();
            builder.ConfigureValidators();
        }

        private static void ConfigureHealthChecks(this WebApplicationBuilder builder)
        {
            builder.Services.AddHealthChecks()
                .AddCheck<MongoHealthCheck>("mongodb");
        }
        private static void ConfigureDependencyInjectionService(this WebApplicationBuilder builder)
        {
            //builder.Services.AddScoped<OwnedGameAccessFilter>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();

            builder.Services.AddHttpClient("NotificationClient", client =>
            {
                client.BaseAddress = new Uri("https://SEU_ENDPOINT_DA_AZURE_FUNCTION");
                client.Timeout = TimeSpan.FromSeconds(10);
            });

        }
        private static void ConfigureDependencyInjectionRepository(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
        }
        private static void ConfigureJwt(this WebApplicationBuilder builder)
        {
            var configuration = builder.Configuration.GetSection("Jwt");

            var issuer = configuration["Issuer"] ?? throw new InvalidOperationException("JWT Issuer not configured");
            var audience = configuration["Audience"] ?? throw new InvalidOperationException("JWT Audience not configured");
            var key = configuration["Key"] ?? throw new InvalidOperationException("JWT Key not configured");

            // Log para debug
            Log.Information("JWT Configuration - Issuer: {Issuer}, Audience: {Audience}, Key Length: {KeyLength}",
                issuer, audience, key.Length);

            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(o =>
                {
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                        ClockSkew = TimeSpan.Zero // Remover tolerância de tempo
                    };

                    o.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            var exception = context.Exception;
                            Log.Error(exception, "JWT Authentication Failed - Exception: {ExceptionType}, Message: {Message}",
                                exception.GetType().Name, exception.Message);

                            // Log detalhado do erro
                            if (exception is SecurityTokenInvalidIssuerException)
                                Log.Error("JWT Error: Invalid Issuer. Expected: {ExpectedIssuer}", issuer);
                            if (exception is SecurityTokenInvalidAudienceException)
                                Log.Error("JWT Error: Invalid Audience. Expected: {ExpectedAudience}", audience);
                            if (exception is SecurityTokenExpiredException)
                                Log.Error("JWT Error: Token has expired");
                            if (exception is SecurityTokenSignatureKeyNotFoundException)
                                Log.Error("JWT Error: Signature key not found");

                            context.Response.ContentType = "application/json";
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            var apiResponse = ApiResponse<string>.Fail("Authentication Failed",
                                [$"JWT validation failed: {exception.Message}"]);
                            var jsonResponse = JsonSerializer.Serialize(apiResponse);

                            return context.Response.WriteAsync(jsonResponse);
                        },
                        OnForbidden = context =>
                        {
                            context.Response.ContentType = "application/json";
                            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                            var apiResponse = ApiResponse<string>.Fail("Authorization Denied", ["You do not have the necessary permissions to access this resource."]);
                            var jsonResponse = JsonSerializer.Serialize(apiResponse);

                            Log.Warning("Forbidden access attempt: {Path}", context.Request.Path);
                            return context.Response.WriteAsync(jsonResponse);
                        },
                        OnChallenge = context =>
                        {
                            context.HandleResponse();
                            context.Response.ContentType = "application/json";
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            var apiResponse = ApiResponse<string>.Fail("Authentication Failed", ["Authentication required or invalid credentials."]);
                            var jsonResponse = JsonSerializer.Serialize(apiResponse);

                            Log.Warning("Unauthorized access attempt: {Path}", context.Request.Path);
                            return context.Response.WriteAsync(jsonResponse);
                        }
                    };
                });

            builder.Services.AddAuthorization();
            //builder.Services.AddScoped<TokenService>();
        }

        private static void ConfigureMongoDbContext(this WebApplicationBuilder builder)
        {
            // Bind das settings
            var mongoSettings = new MongoSettings();
            builder.Configuration.GetSection("MongoDb").Bind(mongoSettings);

            builder.Services.AddSingleton(mongoSettings);

            // Cliente global
            builder.Services.AddSingleton<IMongoClient>(sp =>
                new MongoClient(mongoSettings.ConnectionString));

            // Database principal (Pagamentos)
            builder.Services.AddScoped(sp =>
            {
                var client = sp.GetRequiredService<IMongoClient>();
                return client.GetDatabase(mongoSettings.Database);
            });
        }

        private static void ConfigureLogMongo(this WebApplicationBuilder builder)
        {
            var mongo = builder.Configuration.GetSection("MongoDb").Get<MongoSettings>();

            var logConnection = $"{mongo.ConnectionString}/{mongo.Logging.Database}";

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.MongoDB(
                    logConnection,
                    collectionName: mongo.Logging.Collection
                )
                .CreateLogger();

            builder.Host.UseSerilog();
        }

        private static void ConfigureSwagger(this WebApplicationBuilder builder)
        {
            builder.Services.AddSwaggerGen(options =>
            {
                options.SchemaFilter<EnumSchemaFilter>();
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "FiapCloudGamesApi",
                    Version = "v1",
                    Description = "API Web ASP.NET Core",
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        In = ParameterLocation.Header,
                        Scheme = "bearer"
                    },
                    Array.Empty<string>()
                }
            });

                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });
        }

        private static void ConfigureValidators(this WebApplicationBuilder builder)
        {
            builder.Services.AddValidatorsFromAssemblyContaining<Program>();

        }

        private static void UseJsonFileConfiguration(this WebApplicationBuilder builder)
        {
            var keysDirectoryPath = Path.Combine(AppContext.BaseDirectory, "dataprotection-keys");
            var keysDirectory = new DirectoryInfo(keysDirectoryPath);

            if (!keysDirectory.Exists)
                keysDirectory.Create();


            builder.Services.AddDataProtection()
                .PersistKeysToFileSystem(keysDirectory)
                .SetApplicationName("FIAP.CloudGames");

            builder.Configuration
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.Secrets.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
        }
    }
}

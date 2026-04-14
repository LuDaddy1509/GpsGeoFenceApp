using System.Text;
using MapApi.Common;
using MapApi.Data;
using MapApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace MapApi.Configuration;

public static class ApiStartupExtensions
{
    public static IServiceCollection AddApiRuntimeOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<ApiRuntimeOptions>()
            .Bind(configuration.GetSection(ApiRuntimeOptions.SectionName))
            .ValidateDataAnnotations();

        services
            .AddOptions<AuthOptions>()
            .Bind(configuration.GetSection(AuthOptions.SectionName))
            .ValidateDataAnnotations();

        return services;
    }

    public static IServiceCollection AddApiDataAccess(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var runtimeOptions = configuration
            .GetSection(ApiRuntimeOptions.SectionName)
            .Get<ApiRuntimeOptions>() ?? new ApiRuntimeOptions();

        var connectionString = ResolveConnectionString(configuration, environment);
        services.AddDbContext<AppDb>(options =>
            options.UseSqlServer(connectionString, sql =>
            {
                sql.EnableRetryOnFailure(
                    runtimeOptions.SqlRetryCount,
                    TimeSpan.FromSeconds(runtimeOptions.SqlRetryDelaySeconds),
                    null);
                sql.CommandTimeout(runtimeOptions.SqlCommandTimeoutSeconds);
            }));

        return services;
    }

    public static IServiceCollection AddApiAuthenticationServices(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var runtimeOptions = configuration
            .GetSection(ApiRuntimeOptions.SectionName)
            .Get<ApiRuntimeOptions>() ?? new ApiRuntimeOptions();

        var authOptions = configuration
            .GetSection(AuthOptions.SectionName)
            .Get<AuthOptions>() ?? new AuthOptions();

        var jwtSecret = ResolveJwtSecret(configuration, environment, runtimeOptions);
        var jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
        services.AddSingleton(jwtKey);

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = jwtKey,
                    ValidateIssuer = true,
                    ValidIssuer = authOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = authOptions.Audience,
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("admin"));
        });

        return services;
    }

    public static IServiceCollection AddApiContracts(this IServiceCollection services)
    {
        services.AddProblemDetails();
        services.AddControllers()
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var details = context.ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .ToDictionary(
                            x => x.Key,
                            x => x.Value!.Errors
                                .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? "Invalid value." : e.ErrorMessage)
                                .ToArray());

                    return new BadRequestObjectResult(new ApiErrorResponse
                    {
                        Code = "validation_error",
                        Message = "Validation failed.",
                        Details = details,
                        TraceId = context.HttpContext.TraceIdentifier
                    });
                };
            });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddCors(options =>
            options.AddDefaultPolicy(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));

        return services;
    }

    public static IServiceCollection AddApiApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<UserRoleService>();
        services.AddScoped<AuthService>();
        services.AddScoped<PoiManagementService>();
        services.AddScoped<MediaStorageService>();
        services.AddScoped<NarrationService>();
        services.AddScoped<HistoryService>();
        services.AddHostedService<TranslationBackgroundService>();

        return services;
    }

    public static IServiceCollection AddApiExternalClients(this IServiceCollection services, IConfiguration configuration)
    {
        var runtimeOptions = configuration
            .GetSection(ApiRuntimeOptions.SectionName)
            .Get<ApiRuntimeOptions>() ?? new ApiRuntimeOptions();

        services.AddHttpClient<TranslatorClient>(http =>
        {
            http.Timeout = TimeSpan.FromSeconds(runtimeOptions.TranslatorHttpTimeoutSeconds);
        });

        return services;
    }

    public static WebApplication UseApiDefaults(this WebApplication app)
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var exception = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;
                var logger = context.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("GlobalException");
                if (exception is not null)
                    logger.LogError(exception, "Unhandled exception.");

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new ApiErrorResponse
                {
                    Code = "internal_error",
                    Message = "An unexpected error occurred.",
                    TraceId = context.TraceIdentifier
                });
            });
        });

        app.UseCors();
        app.UseStaticFiles();
        app.UseAuthentication();
        app.UseAuthorization();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapGet("/health", () => Results.Ok(new { ok = true, time = DateTime.UtcNow }));
        app.MapGet("/admin", context =>
        {
            context.Response.Redirect("/admin/index.html");
            return Task.CompletedTask;
        });

        return app;
    }

    private static string ResolveConnectionString(IConfiguration configuration, IHostEnvironment environment)
    {
        var configured = configuration.GetConnectionString("Default");
        if (!string.IsNullOrWhiteSpace(configured))
            return configured;

        if (!environment.IsDevelopment())
            throw new InvalidOperationException("Missing ConnectionStrings:Default configuration.");

        throw new InvalidOperationException("Missing ConnectionStrings:Default configuration for development.");
    }

    private static string ResolveJwtSecret(
        IConfiguration configuration,
        IHostEnvironment environment,
        ApiRuntimeOptions runtimeOptions)
    {
        var configured = configuration["Jwt:Secret"];
        if (!string.IsNullOrWhiteSpace(configured) && !string.Equals(configured, "CHANGE_ME", StringComparison.Ordinal))
            return configured;

        if (!environment.IsDevelopment() || !runtimeOptions.AllowDevelopmentJwtFallback)
            throw new InvalidOperationException("Missing Jwt:Secret configuration.");

        throw new InvalidOperationException("Missing Jwt:Secret configuration for development.");
    }
}

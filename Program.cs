using System;
using System.IO;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using DotnetCoreExampleApi.Constant;
using DotnetCoreExampleApi.Data;
using DotnetCoreExampleApi.Data.Seeds;
using DotnetCoreExampleApi.Extensions;
using DotnetCoreExampleApi.Filters;
using DotnetCoreExampleApi.Handlers;
using DotnetCoreExampleApi.Handlers.Deposit;
using DotnetCoreExampleApi.Handlers.Interfaces;
using DotnetCoreExampleApi.Handlers.Withdrawal;
using DotnetCoreExampleApi.Middleware;
using DotnetCoreExampleApi.Proxies.Demo;
using DotnetCoreExampleApi.Services;
using DotnetCoreExampleApi.Services.Interfaces;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

string env = builder.Environment.EnvironmentName;

builder.Configuration.SetBasePath(Directory.GetCurrentDirectory());
builder.Configuration.AddJsonFile("appsettings.json", false, false);
builder.Configuration.AddJsonFile($"appsettings.{env}.json", false, true);
builder.Configuration.AddEnvironmentVariables();

builder.Logging.ClearProviders();

builder.Host.UseSerilog((context, configuration) =>
{
    configuration.Enrich.FromLogContext();
    configuration.Enrich.WithProperty("BusinessDomain", "Demo");
    configuration.Enrich.WithProperty("Host", Environment.MachineName);
    configuration.Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName);
    configuration.Enrich.WithProperty("Cloud", context.Configuration["CloudProvider"]!);
    configuration.Enrich.WithProperty("BuildId", context.Configuration["BuildId"]!);
    configuration.MinimumLevel.Override("Microsoft", LogEventLevel.Error);
    configuration.MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information);
    configuration.MinimumLevel.Override("System.Net.Http.HttpClient", LogEventLevel.Warning);
    configuration.ReadFrom.Configuration(context.Configuration);
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxConcurrentConnections = null;
    options.Limits.MaxConcurrentUpgradedConnections = null;
});

builder.Services.AddApiVersioning(options => { options.ReportApiVersions = true; });

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = AppConstant.AppVersion;
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddControllers(options => { options.Filters.Add(new ProblemDetailsExceptionFilter()); }).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddHeaderPropagation(o =>
{
    o.Headers.Add("ClientId", "DemoClientId");
});

builder.Services.AddValidatorsFromAssemblyContaining<Program>().AddFluentValidationAutoValidation(fv => fv.DisableDataAnnotationsValidation = true);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwagger();

builder.Services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "Liveness", "Readiness" });

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.Authority = builder.Configuration["IdentityServer:Authority"];
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization(o =>
{
    o.DefaultPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme).Build();
});

builder.Services.AddDbContext<DemoDbContext>(options => { options.UseSqlServer(builder.Configuration.GetConnectionString("DemoCnn")!, sqlOptions => { sqlOptions.EnableRetryOnFailure(3); }); });

builder.Services.AddHttpClient<IDemoApiProxy, DemoApiProxy>(c =>
{
    c.BaseAddress = new Uri(builder.Configuration["DemoApiUrl"]!);
    c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
}).AddHeaderPropagation().AddPolicyHandler(HttpPolicies.GetRetryPolicy).AddPolicyHandler(HttpPolicies.GetCircuitBreakerPolicy());

builder.Services.AddRedis(builder.Configuration);

builder.Services.AddMasTransit(builder.Configuration);

builder.Services.AddBackgroundService();

builder.Services.AddScoped<IOutboxMessagePublisherService, OutboxMessagePublisherService>();

builder.Services.AddScoped<ITransactionHandler, DepositTransactionHandler>();
builder.Services.AddScoped<ITransactionHandler, WithdrawalTransactionHandler>();
builder.Services.AddScoped<ITransactionHandlerResolver, TransactionHandlerResolver>();

var app = builder.Build();

await app.MigrateWithData();

ForwardedHeadersOptions forwardedHeadersOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};

forwardedHeadersOptions.KnownNetworks.Clear();

forwardedHeadersOptions.KnownProxies.Clear();

app.UseForwardedHeaders(forwardedHeadersOptions);

string[] supportedUiCultures = { "en","tr" };

RequestLocalizationOptions localizationOptions = new RequestLocalizationOptions().AddSupportedUICultures(supportedUiCultures);

app.UseRequestLocalization(localizationOptions);

app.UsePathBase("/demo");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/demo/swagger/v1/swagger.json", "Demo API v1");
    });
}

app.UseMiddleware<LoggingMiddleware>();

app.UseMiddleware<CorrelationIdMiddleware>();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/live", new HealthCheckOptions() {Predicate = p=>p.Tags.Contains("Liveness")});

app.MapHealthChecks("/ready", new HealthCheckOptions() {Predicate = p=>p.Tags.Contains("Readiness")});

await app.RunAsync();
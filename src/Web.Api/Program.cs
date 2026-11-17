using VohuManah.Application;
using HealthChecks.UI.Client;
using VohuManah.Infrastructure;
using VohuManah.Infrastructure.Database;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using System.Reflection;
using VohuManah.Web.Api;
using VohuManah.Web.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureKestrel();

builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddSwaggerGenWithAuth();

builder.Services
    .AddApplication()
    .AddPresentation()
    .AddInfrastructure(builder.Configuration);

builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());

var app = builder.Build();

app.UseRequestContextLogging();

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

app.UseCors("CorsPolicy");

app.UseUploads();

app.UseCookieTokenAuthentication();

app.UseRequestLimits();

app.UseCookieTokenAuthentication();

app.UseCsrfProtection();

app.UseAuthentication();

app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

RouteGroupBuilder secured = app.MapGroup("/").RequireAuthorization();
app.MapEndpoints(secured);

app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Services.ApplyMigrations(app.Environment.IsDevelopment());

app.MapControllers();

await app.RunAsync();

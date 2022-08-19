using System.Diagnostics;
using Example.App;
using Microsoft.ApplicationInsights.Extensibility;
using Serilog;
using Serilog.Configuration;
using kr.bbon.AspNetCore.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Serilog.Sinks.ApplicationInsights.TelemetryConverters;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Serilog.Debugging.SelfLog.Enable(message => Debug.WriteLine(message));

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<ApplicationInsightsServiceOptions>()
    .Configure<IConfiguration>((options, configuration) =>
    {
        configuration.GetSection("ApplicationInsights").Bind(options);
    });

builder.Host.UseSerilog((context, services, loggerConfiguration) =>
{
    var telemetryConfiguration = services.GetRequiredService<TelemetryConfiguration>();

    loggerConfiguration
    .MinimumLevel.Information()
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .Enrich.With<StageEnvironmentEnricher>()
    .WriteTo.Console()
    ;

    if (!string.IsNullOrEmpty(telemetryConfiguration.InstrumentationKey))
    {
        loggerConfiguration.WriteTo.ApplicationInsights(new CustomPropertiesTelemetryConverter());
    }

})
;

// Add services to the container.

// The following line enables Application Insights telemetry collection.
builder.Services.AddApplicationInsightsTelemetry();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<Example.App.AppExceptionFilter>();
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
var defaultVersion = new ApiVersion(1, 0);
builder.Services.AddApiVersioningAndSwaggerGen(default);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI();
    app.UseSwaggerUIWithApiVersioning();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "An unhandled exception occured during bootstrapping");
}
finally
{
    Log.CloseAndFlush();
}


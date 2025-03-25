using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Debugging;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Microsoft.OpenApi.Models;

// Initialiser Serilog og opsæt logger
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.Seq("http://seq:5341") // Opdatér URL'en, hvis det kræves
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "HelloWorld.API")
    .CreateBootstrapLogger();

Log.Information("Starting up!");

// Konfigurer builder
var builder = WebApplication.CreateBuilder(args);

// Konfigurer Serilog
builder.Host.UseSerilog((ctx, lc) =>
{
    lc.WriteTo.Console()
      .WriteTo.Seq("http://seq:5341") // URL til Seq
      .Enrich.FromLogContext()
      .Enrich.WithProperty("Application", "HelloWorld.API");

    // Aktiver debug-self-log til at opdage problemer under initialisering
    SelfLog.Enable(msg =>
    {
        Console.WriteLine($"Serilog SelfLog: {msg}");
    });
});

// Tilføj services
builder.Services.AddControllers(); // Tilføj controllers
builder.Services.AddCors();  // CORS support
builder.Services.AddEndpointsApiExplorer(); // Til API endpoints
builder.Services.AddSwaggerGen(c => // Swagger-konfiguration
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "HelloWorld API",
        Version = "v1",
        Description = "En simpel API-dokumentation"
    });
});

// Konfigurer OpenTelemetry
builder.Services.AddOpenTelemetryTracing(tracerProviderBuilder =>
{
    tracerProviderBuilder
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("HelloWorld.API"))
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddSource("API") // Brug til custom events med ActivitySource
        .AddZipkinExporter(o =>
        {
            o.Endpoint = new Uri("http://zipkin:9411/api/v2/spans"); // Zipkin URL
        });
});

var app = builder.Build();

// Middleware-pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(policy =>
    policy.AllowAnyMethod()
          .AllowAnyHeader()
          .AllowAnyOrigin()); // Overvej sikker opsætning i produktion

app.UseHttpsRedirection();
app.UseSerilogRequestLogging(); // Serilog til at logge indkommende requests
app.UseAuthorization();

app.MapControllers(); // Map controllers

try
{
    Log.Information("Starting the web application.");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "An unhandled exception occurred while starting the application.");
}
finally
{
    Log.CloseAndFlush();
}
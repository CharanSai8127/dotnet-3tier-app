using Microsoft.Extensions.Options;
using DotNetMongoCRUDApp.Models;
using DotNetMongoCRUDApp.Services;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// MongoDB settings
builder.Services.Configure<MongoDBSettings>(
    builder.Configuration.GetSection(nameof(MongoDBSettings)));

builder.Services.AddSingleton<IMongoDBSettings>(sp =>
    sp.GetRequiredService<IOptions<MongoDBSettings>>().Value);

builder.Services.AddSingleton<ProductService>();


//  ------------------------------
//  OpenTelemetry Metrics Pipeline
//  ------------------------------
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource =>
        resource.AddService("dotnetmongocrudapp"))
    .WithMetrics(metrics =>
    {
        metrics.AddAspNetCoreInstrumentation();
        metrics.AddHttpClientInstrumentation();
        metrics.AddRuntimeInstrumentation();

        // ✅ NEW: Correct Prometheus Exporter for .NET 8
        metrics.AddPrometheusHttpListener();
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Expose app on port 5035
app.Urls.Add("http://0.0.0.0:5035");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// ------------------------------
// Prometheus Scrape Endpoint
// ------------------------------
app.MapPrometheusScrapingEndpoint("/metrics");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

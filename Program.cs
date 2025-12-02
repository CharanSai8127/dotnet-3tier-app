using Microsoft.Extensions.Options;
using DotNetMongoCRUDApp.Models;
using DotNetMongoCRUDApp.Services;
using Prometheus; // <-- Add this

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// MongoDB settings
builder.Services.Configure<MongoDBSettings>(
    builder.Configuration.GetSection(nameof(MongoDBSettings)));

builder.Services.AddSingleton<IMongoDBSettings>(sp =>
    sp.GetRequiredService<IOptions<MongoDBSettings>>().Value);

builder.Services.AddSingleton<ProductService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Listen on port 5035
app.Urls.Add("http://0.0.0.0:5035");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 🔥 PROMETHEUS MIDDLEWARE HERE
app.UseHttpMetrics();        // Collect per-request metrics
app.MapMetrics("/metrics");  // Expose metrics endpoint

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

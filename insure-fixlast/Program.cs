using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using insure_fixlast.Data;
using insure_fixlast.Models;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<insure_fixlastContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("insure_fixlastContext") ?? throw new InvalidOperationException("Connection string 'insure_fixlastContext' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache(); // Add distributed memory cache service
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set timeout for the session
    options.Cookie.HttpOnly = true; // Ensure the session cookie is only accessible via HTTP
    options.Cookie.IsEssential = true; // Mark the session cookie as essential, cannot be declined
});

var app = builder.Build();

// Apply migrations and seed data at startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Apply any pending migrations
        var context = services.GetRequiredService<insure_fixlastContext>();
        context.Database.Migrate();

        // Initialize seed data
        SeedData.Initialize(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); // Add this line to use session middleware

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

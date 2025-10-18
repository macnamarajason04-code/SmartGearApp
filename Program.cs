using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmartGearApp.Services;
using SmartGearApp.Filters;
using SmartGearApp.Models;
using SmartGearApp.Hubs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IRequestLogger, ConsoleRequestLogger>();
builder.Services.AddScoped<LogActionFilter>();
builder.Services.AddScoped<AuthorizeActionFilter>();

builder.Services.AddMemoryCache();
builder.Services.AddSignalR();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<IRequestLogger>();
    await logger.LogAsync(context.Request);
    await next();
});

app.Use(async (context, next) =>
{
    var start = DateTime.Now;
    await next();
    var duration = DateTime.Now - start;
    Console.WriteLine($"[Performance] {context.Request.Path} took {duration.TotalMilliseconds} ms");
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Product}/{action=Index}/{id?}");

app.MapControllers();

app.MapHub<ProductHub>("/productHub");

app.MapRazorPages();

app.Run();

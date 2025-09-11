using Knapsak_CFTW.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Ajoute les services nécessaires pour la session
builder.Services.AddDistributedMemoryCache(); // Stocke la session en mémoire (obligatoire)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(1); // Expire après 30 minutes d'inactivité
    options.Cookie.HttpOnly = true; // Sécurise les cookies
    options.Cookie.IsEssential = true; // Indispensable pour le fonctionnement même si les cookies sont restreints
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<SessionService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseStaticFiles();

// Active la session AVANT `UseRouting()`
app.UseSession();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Items}/{action=Index}"
);

app.Run();
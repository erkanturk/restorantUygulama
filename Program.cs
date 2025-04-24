using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using RestoranUygulama.Models;
using RestoranUygulama.DataContext; // Model ve DbContext s�n�flar�n�n namespace'i

var builder = WebApplication.CreateBuilder(args);

// Veritaban� ba�lant�s�n� yap�land�rma (MSSQL ba�lant� dizesi appsettings.json'dan geliyor)
builder.Services.AddDbContext<RestoranContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Session y�netimini ekleme (Garson oturum kontrol� i�in)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Oturum s�resi 30 dakika
    options.Cookie.HttpOnly = true; // Cookie'ye yaln�zca sunucu eri�ebilir
    options.Cookie.IsEssential = true; // �erezler gerekli olarak i�aretlenir
});

// MVC hizmetlerini ekleme (Controller ve View'lar� kullanmak i�in)
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Veritaban� ba�lang�� verilerini ekleme (10 masa gibi seed data)
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<RestoranContext>();
    DbInitializer.Initialize(context); // Ba�lang�� verilerini ekleyen metod (daha �nce tan�mlanm��t�)
}

// HTTP istek hatt�n� yap�land�rma (middleware pipeline)
if (!app.Environment.IsDevelopment())
{
    // Geli�tirme ortam�nda de�ilse hata sayfas� g�ster
    app.UseExceptionHandler("/Home/Error");
    // HSTS (HTTP Strict Transport Security) ayar�, �retim ortam� i�in
    app.UseHsts();
}

// HTTPS y�nlendirmesi (g�venlik i�in)
app.UseHttpsRedirection();

// Statik dosyalar� kullanma (CSS, JS, resimler vb.)
app.UseStaticFiles();

// Routing (URL y�nlendirme) middleware'ini ekleme
app.UseRouting();

// Session middleware'ini ekleme (oturum y�netimi i�in)
app.UseSession();

// Yetkilendirme middleware'ini ekleme (gelecekte authentication i�in kullan�labilir)
app.UseAuthorization();

// Varsay�lan controller rotas�n� tan�mlama
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}"); // Varsay�lan sayfa olarak Login sayfas�n� ayarlad�k

// Uygulamay� �al��t�rma
app.Run();

// DbInitializer s�n�f� (ba�lang�� verilerini eklemek i�in, daha �nce tan�mlanm��t�)

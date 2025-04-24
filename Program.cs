using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using RestoranUygulama.Models; // Model ve DbContext sýnýflarýnýn namespace'i

var builder = WebApplication.CreateBuilder(args);

// Veritabaný baðlantýsýný yapýlandýrma (MSSQL baðlantý dizesi appsettings.json'dan geliyor)
builder.Services.AddDbContext<RestoranContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Session yönetimini ekleme (Garson oturum kontrolü için)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Oturum süresi 30 dakika
    options.Cookie.HttpOnly = true; // Cookie'ye yalnýzca sunucu eriþebilir
    options.Cookie.IsEssential = true; // Çerezler gerekli olarak iþaretlenir
});

// MVC hizmetlerini ekleme (Controller ve View'larý kullanmak için)
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Veritabaný baþlangýç verilerini ekleme (10 masa gibi seed data)
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<RestoranContext>();
    DbInitializer.Initialize(context); // Baþlangýç verilerini ekleyen metod (daha önce tanýmlanmýþtý)
}

// HTTP istek hattýný yapýlandýrma (middleware pipeline)
if (!app.Environment.IsDevelopment())
{
    // Geliþtirme ortamýnda deðilse hata sayfasý göster
    app.UseExceptionHandler("/Home/Error");
    // HSTS (HTTP Strict Transport Security) ayarý, üretim ortamý için
    app.UseHsts();
}

// HTTPS yönlendirmesi (güvenlik için)
app.UseHttpsRedirection();

// Statik dosyalarý kullanma (CSS, JS, resimler vb.)
app.UseStaticFiles();

// Routing (URL yönlendirme) middleware'ini ekleme
app.UseRouting();

// Session middleware'ini ekleme (oturum yönetimi için)
app.UseSession();

// Yetkilendirme middleware'ini ekleme (gelecekte authentication için kullanýlabilir)
app.UseAuthorization();

// Varsayýlan controller rotasýný tanýmlama
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}"); // Varsayýlan sayfa olarak Login sayfasýný ayarladýk

// Uygulamayý çalýþtýrma
app.Run();

// DbInitializer sýnýfý (baþlangýç verilerini eklemek için, daha önce tanýmlanmýþtý)
public static class DbInitializer
{
    public static void Initialize(RestoranContext context)
    {
        context.Database.EnsureCreated(); // Veritabaný yoksa oluþtur

        // Eðer Masalar tablosu boþsa, 10 masa ekle
        if (!context.Masalar.Any())
        {
            for (int i = 1; i <= 10; i++)
            {
                context.Masalar.Add(new Masa { MasaNumarasi = i, DoluMu = false });
            }
            context.SaveChanges();
        }

        // Test amaçlý bir garson kullanýcýsý ekle (þifre düz metin, üretimde hash'lenmeli)
        if (!context.Kullanicilar.Any())
        {
            context.Kullanicilar.Add(new Kullanici
            {
                KullaniciAdi = "garson1",
                Sifre = "12345",
                Rol = "Garson"
            });
            context.SaveChanges();
        }
    }
}

using RestoranUygulama.Models;

namespace RestoranUygulama.DataContext
{
    public static class DbInitializer
    {
        public static void Initialize(RestoranContext context)
        {
            context.Database.EnsureCreated(); // Veritabanı yoksa oluştur

            // Eğer Masalar tablosu boşsa, 10 masa ekle
            if (!context.Masalar.Any())
            {
                for (int i = 1; i <= 10; i++)
                {
                    context.Masalar.Add(new Masa { MasaNumarasi = i, DoluMu = false });
                }
                context.SaveChanges();
            }

            // Test amaçlı bir garson kullanıcısı ekle (şifre düz metin, üretimde hash'lenmeli)
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
}

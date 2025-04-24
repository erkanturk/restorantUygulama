using RestoranUygulama.Models;

namespace RestoranUygulama.DataContext
{
    public static class DbInitializer
    {
        public static void Initialize(RestoranContext context)
        {
            context.Database.EnsureCreated();

            if (!context.Masalar.Any())
            {
                for (int i = 1; i <= 10; i++)
                {
                    context.Masalar.Add(new Masa { MasaNumarasi = i, DoluMu = false });
                }
                context.SaveChanges();
            }
        }
    }
}

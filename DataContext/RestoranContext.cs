using Microsoft.EntityFrameworkCore;
using RestoranUygulama.Models;
using System.Collections.Generic;

public class RestoranContext : DbContext
{
    public RestoranContext(DbContextOptions<RestoranContext> options) : base(options)
    {
    }

    public DbSet<Masa> Masalar { get; set; }
    public DbSet<Rezervasyon> Rezervasyonlar { get; set; }
    public DbSet<Kullanici> Kullanicilar { get; set; }
}

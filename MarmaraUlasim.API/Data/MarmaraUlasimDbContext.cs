using Microsoft.EntityFrameworkCore;
using MarmaraUlasim.API.Models;

namespace MarmaraUlasim.API.Data;

public class MarmaraUlasimDbContext : DbContext
{
    public MarmaraUlasimDbContext(
        DbContextOptions<MarmaraUlasimDbContext> options)
        : base(options)
    {
    }

    public DbSet<Il> Iller => Set<Il>();

    public DbSet<Ilce> Ilceler => Set<Ilce>();

    public DbSet<Mahalle> Mahalleler => Set<Mahalle>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // İl - İlçe ilişkisi
        modelBuilder.Entity<Ilce>()
            .HasOne(x => x.Il)
            .WithMany()
            .HasForeignKey(x => x.IlId)
            .OnDelete(DeleteBehavior.Cascade);

        // TurkiyeAPI ilçe ID'si benzersiz olsun
        modelBuilder.Entity<Ilce>()
            .HasIndex(x => x.ApiId)
            .IsUnique();

        // İlçe - Mahalle ilişkisi
        modelBuilder.Entity<Mahalle>()
            .HasOne(x => x.Ilce)
            .WithMany()
            .HasForeignKey(x => x.IlceId)
            .OnDelete(DeleteBehavior.Cascade);

        // TurkiyeAPI mahalle ID'si benzersiz olsun
        modelBuilder.Entity<Mahalle>()
            .HasIndex(x => x.ApiId)
            .IsUnique();

        // Marmara Bölgesi illeri
        modelBuilder.Entity<Il>().HasData(
            new Il { Id = 1, Ad = "İstanbul", PlakaKodu = 34 },
            new Il { Id = 2, Ad = "Edirne", PlakaKodu = 22 },
            new Il { Id = 3, Ad = "Kırklareli", PlakaKodu = 39 },
            new Il { Id = 4, Ad = "Tekirdağ", PlakaKodu = 59 },
            new Il { Id = 5, Ad = "Çanakkale", PlakaKodu = 17 },
            new Il { Id = 6, Ad = "Balıkesir", PlakaKodu = 10 },
            new Il { Id = 7, Ad = "Bursa", PlakaKodu = 16 },
            new Il { Id = 8, Ad = "Yalova", PlakaKodu = 77 },
            new Il { Id = 9, Ad = "Kocaeli", PlakaKodu = 41 },
            new Il { Id = 10, Ad = "Sakarya", PlakaKodu = 54 },
            new Il { Id = 11, Ad = "Bilecik", PlakaKodu = 11 }
        );
    }
}
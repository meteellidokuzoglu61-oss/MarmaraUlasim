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

    public DbSet<Durak> Duraklar => Set<Durak>();
    public DbSet<Hat> Hatlar => Set<Hat>();
    public DbSet<Sefer> Seferler => Set<Sefer>();
    public DbSet<SeferDurak> SeferDuraklar => Set<SeferDurak>();
    public DbSet<GuzergahNoktasi> GuzergahNoktalari => Set<GuzergahNoktasi>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ============================================================
        // İL - İLÇE
        // ============================================================

        modelBuilder.Entity<Ilce>()
            .HasOne(x => x.Il)
            .WithMany()
            .HasForeignKey(x => x.IlId)
            .OnDelete(DeleteBehavior.Cascade);

        // TurkiyeAPI ilçe ID'si benzersiz olsun
        modelBuilder.Entity<Ilce>()
            .HasIndex(x => x.ApiId)
            .IsUnique();


        // ============================================================
        // İLÇE - MAHALLE
        // ============================================================

        modelBuilder.Entity<Mahalle>()
            .HasOne(x => x.Ilce)
            .WithMany()
            .HasForeignKey(x => x.IlceId)
            .OnDelete(DeleteBehavior.Cascade);

        // TurkiyeAPI mahalle ID'si benzersiz olsun
        modelBuilder.Entity<Mahalle>()
            .HasIndex(x => x.ApiId)
            .IsUnique();


        // ============================================================
        // İLÇE - DURAK
        // ============================================================

        modelBuilder.Entity<Durak>()
            .HasOne(x => x.Ilce)
            .WithMany()
            .HasForeignKey(x => x.IlceId)
            .OnDelete(DeleteBehavior.SetNull);


        // ============================================================
        // MAHALLE - DURAK
        // ============================================================

        modelBuilder.Entity<Durak>()
            .HasOne(x => x.Mahalle)
            .WithMany()
            .HasForeignKey(x => x.MahalleId)
            .OnDelete(DeleteBehavior.SetNull);


        // Durak kodu benzersiz olsun
        modelBuilder.Entity<Durak>()
    .HasIndex(x => new
    {
        x.Kaynak,
        x.DurakKodu
    })
    .IsUnique();

    modelBuilder.Entity<Sefer>()
    .HasOne(x => x.Hat)
    .WithMany()
    .HasForeignKey(x => x.HatKodu)
    .HasPrincipalKey(x => x.HatKodu)
    .OnDelete(DeleteBehavior.Cascade);

modelBuilder.Entity<Sefer>()
    .HasIndex(x => new
    {
        x.Kaynak,
        x.SeferKodu
    })
    .IsUnique();

    modelBuilder.Entity<SeferDurak>()
    .HasOne(x => x.Sefer)
    .WithMany()
    .HasForeignKey(x => x.SeferKodu)
    .HasPrincipalKey(x => x.SeferKodu)
    .OnDelete(DeleteBehavior.Cascade);

modelBuilder.Entity<SeferDurak>()
    .HasOne(x => x.Durak)
    .WithMany()
    .HasForeignKey(x => x.DurakKodu)
    .HasPrincipalKey(x => x.DurakKodu)
    .OnDelete(DeleteBehavior.Cascade);

modelBuilder.Entity<SeferDurak>()
    .HasIndex(x => new
    {
        x.SeferKodu,
        x.DurakSirasi
    })
    .IsUnique();
modelBuilder.Entity<GuzergahNoktasi>()
    .HasIndex(x => new
    {
        x.Kaynak,
        x.ShapeId,
        x.Sira
    })
    .IsUnique();

        // ============================================================
        // MARMARA BÖLGESİ İLLERİ
        // ============================================================

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
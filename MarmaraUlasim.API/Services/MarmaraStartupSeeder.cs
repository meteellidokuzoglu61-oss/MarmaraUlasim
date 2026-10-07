using Microsoft.EntityFrameworkCore;
using MarmaraUlasim.API.Data;
using MarmaraUlasim.API.Models;

namespace MarmaraUlasim.API.Services;

public class MarmaraStartupSeeder : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<MarmaraStartupSeeder> _logger;

    public MarmaraStartupSeeder(
        IServiceScopeFactory scopeFactory,
        ILogger<MarmaraStartupSeeder> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);

        try
        {
            using var scope = _scopeFactory.CreateScope();

            var db = scope.ServiceProvider
                .GetRequiredService<MarmaraUlasimDbContext>();

            var service = scope.ServiceProvider
                .GetRequiredService<TurkiyeApiService>();

            if (!await db.Database.CanConnectAsync(stoppingToken))
            {
                _logger.LogWarning(
                    "PostgreSQL baglantisi kurulamadi; veri aktarimi atlandi.");

                return;
            }

            await MarmaraIlleriniKontrolEtAsync(
                db,
                stoppingToken);

            var ilceSayisi = await db.Ilceler
                .CountAsync(stoppingToken);

            if (ilceSayisi == 0)
            {
                var adet = await service.IlceleriAktarAsync();

                _logger.LogInformation(
                    "{Adet} ilce aktarildi.",
                    adet);
            }

            var mahalleSayisi = await db.Mahalleler
                .CountAsync(stoppingToken);

            if (mahalleSayisi == 0)
            {
                var adet = await service.MahalleleriAktarAsync();

                _logger.LogInformation(
                    "{Adet} mahalle aktarildi.",
                    adet);
            }

            _logger.LogInformation(
                "Marmara cografi veri kontrolu tamamlandi.");
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Otomatik Marmara veri aktarimi basarisiz.");
        }
    }

    private static async Task MarmaraIlleriniKontrolEtAsync(
        MarmaraUlasimDbContext db,
        CancellationToken cancellationToken)
    {
        var mevcutIller = await db.Iller
            .AsNoTracking()
            .Select(x => x.PlakaKodu)
            .ToListAsync(cancellationToken);

        var marmaraIlleri = new[]
        {
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
        };

        var eksikIller = marmaraIlleri
            .Where(x => !mevcutIller.Contains(x.PlakaKodu))
            .ToList();

        if (eksikIller.Count == 0)
        {
            return;
        }

        await db.Iller.AddRangeAsync(
            eksikIller,
            cancellationToken);

        await db.SaveChangesAsync(cancellationToken);
    }
}

using Microsoft.EntityFrameworkCore;
using MarmaraUlasim.API.Data;

namespace MarmaraUlasim.API.Services;

public class MarmaraStartupSeeder : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<MarmaraStartupSeeder> _logger;

    public MarmaraStartupSeeder(IServiceScopeFactory scopeFactory, ILogger<MarmaraStartupSeeder> logger)
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
            var db = scope.ServiceProvider.GetRequiredService<MarmaraUlasimDbContext>();
            var service = scope.ServiceProvider.GetRequiredService<TurkiyeApiService>();

            if (!await db.Database.CanConnectAsync(stoppingToken))
            {
                _logger.LogWarning("PostgreSQL baglantisi kurulamadi; veri aktarimi atlandi.");
                return;
            }

            var ilceSayisi = await db.Ilceler.CountAsync(stoppingToken);
            if (ilceSayisi == 0)
            {
                var adet = await service.IlceleriAktarAsync();
                _logger.LogInformation("{Adet} ilce aktarildi.", adet);
            }

            var mahalleSayisi = await db.Mahalleler.CountAsync(stoppingToken);
            if (mahalleSayisi == 0)
            {
                var adet = await service.MahalleleriAktarAsync();
                _logger.LogInformation("{Adet} mahalle aktarildi.", adet);
            }

            _logger.LogInformation("Marmara cografi veri kontrolu tamamlandi.");
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Otomatik Marmara veri aktarimi basarisiz.");
        }
    }
}
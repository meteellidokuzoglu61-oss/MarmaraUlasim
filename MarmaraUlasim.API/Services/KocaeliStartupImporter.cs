using System.IO.Compression;
using MarmaraUlasim.API.Data;
using Microsoft.EntityFrameworkCore;

namespace MarmaraUlasim.API.Services;

public class KocaeliStartupImporter : BackgroundService
{
    private const string FeedUrl =
        "https://github.com/Egezenn/kk-gtfs/raw/main/data/kocaeli.zip";

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<KocaeliStartupImporter> _logger;

    public KocaeliStartupImporter(
        IServiceScopeFactory scopeFactory,
        IHttpClientFactory httpClientFactory,
        ILogger<KocaeliStartupImporter> logger)
    {
        _scopeFactory = scopeFactory;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

        try
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MarmaraUlasimDbContext>();

            if (!await db.Database.CanConnectAsync(stoppingToken))
            {
                _logger.LogWarning("Kocaeli GTFS aktarimi atlandi: PostgreSQL baglantisi yok.");
                return;
            }

            var mevcutHat = await db.Hatlar
                .AnyAsync(x => x.Kaynak == "KentKart-Kocaeli", stoppingToken);

            if (mevcutHat)
            {
                _logger.LogInformation("Kocaeli GTFS verileri zaten mevcut.");
                return;
            }

            var tempRoot = Path.Combine(Path.GetTempPath(), "MarmaraUlasim", "kocaeli");
            Directory.CreateDirectory(tempRoot);

            var zipPath = Path.Combine(tempRoot, "kocaeli.zip");
            var extractPath = Path.Combine(tempRoot, "gtfs");

            if (Directory.Exists(extractPath))
                Directory.Delete(extractPath, true);

            Directory.CreateDirectory(extractPath);

            _logger.LogInformation("Kocaeli GTFS verisi indiriliyor...");
            var client = _httpClientFactory.CreateClient();

            await using (var source = await client.GetStreamAsync(FeedUrl, stoppingToken))
            await using (var target = File.Create(zipPath))
            {
                await source.CopyToAsync(target, stoppingToken);
            }

            ZipFile.ExtractToDirectory(zipPath, extractPath, true);

            var gtfsFolder = FindGtfsFolder(extractPath);

            if (gtfsFolder == null)
            {
                _logger.LogWarning("Kocaeli GTFS arsivinde gerekli dosyalar bulunamadi.");
                return;
            }

            var gtfs = scope.ServiceProvider.GetRequiredService<KocaeliGtfsService>();

            var durak = await gtfs.DuraklariAktarAsync(gtfsFolder);
            var hat = await gtfs.HatlariAktarAsync(gtfsFolder);
            var sefer = await gtfs.SeferleriAktarAsync(gtfsFolder);
            var seferDurak = await gtfs.SeferDuraklariniAktarAsync(gtfsFolder);
            var guzergah = await gtfs.GuzergahNoktalariniAktarAsync(gtfsFolder);
            var shape = await gtfs.SeferShapeIdleriniAktarAsync(gtfsFolder);

            _logger.LogInformation(
                "Kocaeli GTFS tamamlandi. Durak: {Durak}, Hat: {Hat}, Sefer: {Sefer}, SeferDurak: {SeferDurak}, Güzergah: {Guzergah}, Shape: {Shape}",
                durak, hat, sefer, seferDurak, guzergah, shape);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kocaeli GTFS otomatik aktarimi basarisiz.");
        }
    }

    private static string? FindGtfsFolder(string root)
    {
        if (File.Exists(Path.Combine(root, "stops.txt")))
            return root;

        var folder = Directory
            .GetDirectories(root, "*", SearchOption.AllDirectories)
            .FirstOrDefault(x =>
                File.Exists(Path.Combine(x, "stops.txt")) &&
                File.Exists(Path.Combine(x, "routes.txt")) &&
                File.Exists(Path.Combine(x, "trips.txt")));

        return folder;
    }
}
using System.Globalization;
using MarmaraUlasim.API.Data;
using MarmaraUlasim.API.Models;
using Microsoft.EntityFrameworkCore;

namespace MarmaraUlasim.API.Services;

public class KocaeliGtfsService
{
    private readonly MarmaraUlasimDbContext _context;

    public KocaeliGtfsService(
        MarmaraUlasimDbContext context)
    {
        _context = context;
    }

    public async Task<int> DuraklariAktarAsync(
        string gtfsKlasoru)
    {
        // Klasör kontrolü
        if (!Directory.Exists(gtfsKlasoru))
        {
            throw new DirectoryNotFoundException(
                $"GTFS klasörü bulunamadı: {gtfsKlasoru}");
        }

        // stops.txt kontrolü
        var stopsFile = Path.Combine(
            gtfsKlasoru,
            "stops.txt");

        if (!File.Exists(stopsFile))
        {
            throw new FileNotFoundException(
                "GTFS içerisinde stops.txt bulunamadı.",
                stopsFile);
        }

        // Kocaeli
        var kocaeli = await _context.Iller
            .FirstOrDefaultAsync(x => x.PlakaKodu == 41);

        if (kocaeli == null)
        {
            throw new Exception(
                "Kocaeli ili veritabanında bulunamadı.");
        }

        // Daha önce aktarılmış Kocaeli durak kodları
        var mevcutKodlar = await _context.Duraklar
            .Where(x => x.Kaynak == "KentKart-Kocaeli")
            .Select(x => x.DurakKodu)
            .ToHashSetAsync();

        var yeniDuraklar = new List<Durak>();

        var satirlar = await File.ReadAllLinesAsync(
            stopsFile);

        if (satirlar.Length <= 1)
        {
            return 0;
        }

        // CSV başlıkları
        var basliklar = ParseCsvLine(satirlar[0]);

        int stopIdIndex =
            basliklar.IndexOf("stop_id");

        int stopNameIndex =
            basliklar.IndexOf("stop_name");

        int latIndex =
            basliklar.IndexOf("stop_lat");

        int lonIndex =
            basliklar.IndexOf("stop_lon");

        if (stopIdIndex < 0 ||
            stopNameIndex < 0 ||
            latIndex < 0 ||
            lonIndex < 0)
        {
            throw new Exception(
                "stops.txt içerisinde gerekli GTFS alanları bulunamadı.");
        }

        foreach (var satir in satirlar.Skip(1))
        {
            if (string.IsNullOrWhiteSpace(satir))
            {
                continue;
            }

            var alanlar = ParseCsvLine(satir);

            int maksimumIndex = Math.Max(
                Math.Max(stopIdIndex, stopNameIndex),
                Math.Max(latIndex, lonIndex));

            if (alanlar.Count <= maksimumIndex)
            {
                continue;
            }

            var stopId =
                alanlar[stopIdIndex].Trim();

            var stopName =
                alanlar[stopNameIndex].Trim();

            if (!double.TryParse(
                    alanlar[latIndex].Trim(),
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out var latitude))
            {
                continue;
            }

            if (!double.TryParse(
                    alanlar[lonIndex].Trim(),
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out var longitude))
            {
                continue;
            }

            if (string.IsNullOrWhiteSpace(stopId) ||
                string.IsNullOrWhiteSpace(stopName))
            {
                continue;
            }

            // Daha önce aktarılmışsa atla
            if (mevcutKodlar.Contains(stopId))
            {
                continue;
            }

            // İlçe/Mahalle şu aşamada bilinmiyor.
            // Koordinatları gerçek GTFS'den alıyoruz.
            yeniDuraklar.Add(new Durak
            {
                DurakKodu = stopId,
                Ad = stopName,
                Kaynak = "KentKart-Kocaeli",
                Enlem = latitude,
                Boylam = longitude,

                IlceId = null,
                MahalleId = null,

                Aktif = true
            });

            mevcutKodlar.Add(stopId);
        }

        if (yeniDuraklar.Count > 0)
        {
            await _context.Duraklar.AddRangeAsync(
                yeniDuraklar);

            await _context.SaveChangesAsync();
        }

        return yeniDuraklar.Count;
    }

    public async Task<int> HatlariAktarAsync(string gtfsKlasoru)
{
    if (!Directory.Exists(gtfsKlasoru))
    {
        throw new DirectoryNotFoundException(
            $"GTFS klasörü bulunamadı: {gtfsKlasoru}");
    }

    var routesFile = Path.Combine(
        gtfsKlasoru,
        "routes.txt");

    if (!File.Exists(routesFile))
    {
        throw new FileNotFoundException(
            "GTFS içerisinde routes.txt bulunamadı.",
            routesFile);
    }

    var mevcutKodlar = await _context.Hatlar
        .Where(x => x.Kaynak == "KentKart-Kocaeli")
        .Select(x => x.HatKodu)
        .ToHashSetAsync();

    var yeniHatlar = new List<Hat>();

    var satirlar = await File.ReadAllLinesAsync(routesFile);

    if (satirlar.Length <= 1)
    {
        return 0;
    }

    var basliklar = ParseCsvLine(satirlar[0]);

    int routeIdIndex =
        basliklar.IndexOf("route_id");

    int routeShortNameIndex =
        basliklar.IndexOf("route_short_name");

    int routeLongNameIndex =
        basliklar.IndexOf("route_long_name");

    int routeTypeIndex =
        basliklar.IndexOf("route_type");

    if (routeIdIndex < 0)
    {
        throw new Exception(
            "routes.txt içerisinde route_id bulunamadı.");
    }

    foreach (var satir in satirlar.Skip(1))
    {
        if (string.IsNullOrWhiteSpace(satir))
        {
            continue;
        }

        var alanlar = ParseCsvLine(satir);

        if (alanlar.Count <= routeIdIndex)
        {
            continue;
        }

        var hatKodu =
            alanlar[routeIdIndex].Trim();

        if (string.IsNullOrWhiteSpace(hatKodu))
        {
            continue;
        }

        if (mevcutKodlar.Contains(hatKodu))
        {
            continue;
        }

        string ad = "";

        if (routeShortNameIndex >= 0 &&
            alanlar.Count > routeShortNameIndex)
        {
            ad = alanlar[routeShortNameIndex].Trim();
        }

        if (string.IsNullOrWhiteSpace(ad) &&
            routeLongNameIndex >= 0 &&
            alanlar.Count > routeLongNameIndex)
        {
            ad = alanlar[routeLongNameIndex].Trim();
        }

        string? aciklama = null;

        if (routeLongNameIndex >= 0 &&
            alanlar.Count > routeLongNameIndex)
        {
            aciklama =
                alanlar[routeLongNameIndex].Trim();
        }

        string? tip = null;

        if (routeTypeIndex >= 0 &&
            alanlar.Count > routeTypeIndex)
        {
            tip = alanlar[routeTypeIndex].Trim();
        }

        yeniHatlar.Add(new Hat
        {
            HatKodu = hatKodu,
            Ad = ad,
            Tip = tip,
            Aciklama = aciklama,
            Kaynak = "KentKart-Kocaeli",
            Aktif = true
        });

        mevcutKodlar.Add(hatKodu);
    }

    if (yeniHatlar.Count > 0)
    {
        await _context.Hatlar.AddRangeAsync(
            yeniHatlar);

        await _context.SaveChangesAsync();
    }

    return yeniHatlar.Count;
}

    private static List<string> ParseCsvLine(
        string line)
    {
        var result = new List<string>();

        bool quoted = false;

        var current =
            new System.Text.StringBuilder();

        foreach (var character in line)
        {
            if (character == '"')
            {
                quoted = !quoted;
                continue;
            }

            if (character == ',' && !quoted)
            {
                result.Add(current.ToString());
                current.Clear();

                continue;
            }

            current.Append(character);
        }

        result.Add(current.ToString());

        return result;
    }
}
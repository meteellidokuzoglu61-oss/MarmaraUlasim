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
        string gtfsDosyaYolu)
    {
        if (!File.Exists(gtfsDosyaYolu))
        {
            throw new FileNotFoundException(
                "GTFS dosyası bulunamadı.",
                gtfsDosyaYolu);
        }

        var stopsFile = Path.Combine(
            gtfsDosyaYolu,
            "stops.txt");

        if (!File.Exists(stopsFile))
        {
            throw new FileNotFoundException(
                "GTFS içerisinde stops.txt bulunamadı.",
                stopsFile);
        }

        var kocaeli = await _context.Iller
            .FirstOrDefaultAsync(x => x.PlakaKodu == 41);

        if (kocaeli == null)
        {
            throw new Exception(
                "Kocaeli ili veritabanında bulunamadı.");
        }

        var ilceler = await _context.Ilceler
            .Where(x => x.IlId == kocaeli.Id)
            .ToListAsync();

        var mevcutDuraklar = await _context.Duraklar
            .Where(x => x.Kaynak == "KentKart-Kocaeli")
            .ToListAsync();

        var mevcutKodlar = mevcutDuraklar
            .Select(x => x.DurakKodu)
            .ToHashSet();

        var yeniDuraklar = new List<Durak>();

        var satirlar = await File.ReadAllLinesAsync(
            stopsFile);

        if (satirlar.Length <= 1)
        {
            return 0;
        }

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

            if (alanlar.Count <= Math.Max(
                    Math.Max(stopIdIndex, stopNameIndex),
                    Math.Max(latIndex, lonIndex)))
            {
                continue;
            }

            var stopId =
                alanlar[stopIdIndex].Trim();

            var stopName =
                alanlar[stopNameIndex].Trim();

            if (!double.TryParse(
                    alanlar[latIndex],
                    System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out var latitude))
            {
                continue;
            }

            if (!double.TryParse(
                    alanlar[lonIndex],
                    System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out var longitude))
            {
                continue;
            }

            if (string.IsNullOrWhiteSpace(stopId) ||
                string.IsNullOrWhiteSpace(stopName))
            {
                continue;
            }

            if (mevcutKodlar.Contains(stopId))
            {
                continue;
            }

            // İlk aşamada koordinattan ilçe bulma yapılmayacak.
            // Kocaeli olduğu için geçici olarak ilçe eşleştirmesi
            // sonraki aşamada yapılacak.
            var ilce = await IlceBulAsync(
                stopName,
                ilceler);

            if (ilce == null)
            {
                continue;
            }

            yeniDuraklar.Add(new Durak
            {
                DurakKodu = stopId,
                Ad = stopName,
                Kaynak = "KentKart-Kocaeli",
                Enlem = latitude,
                Boylam = longitude,
                IlceId = ilce.Id,
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

    private static async Task<Ilce?> IlceBulAsync(
        string durakAdi,
        List<Ilce> ilceler)
    {
        await Task.CompletedTask;

        foreach (var ilce in ilceler)
        {
            if (durakAdi.Contains(
                    ilce.Ad,
                    StringComparison.OrdinalIgnoreCase))
            {
                return ilce;
            }
        }

        return null;
    }

    private static List<string> ParseCsvLine(
        string line)
    {
        var result = new List<string>();

        bool quoted = false;
        var current = new System.Text.StringBuilder();

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
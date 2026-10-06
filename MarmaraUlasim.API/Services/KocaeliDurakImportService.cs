using MarmaraUlasim.API.Data;
using MarmaraUlasim.API.Models;
using Microsoft.EntityFrameworkCore;

namespace MarmaraUlasim.API.Services;

public class KocaeliDurakImportService
{
    private readonly MarmaraUlasimDbContext _context;

    public KocaeliDurakImportService(
        MarmaraUlasimDbContext context)
    {
        _context = context;
    }

    public async Task<int> DuraklariAktarAsync(
        IEnumerable<KocaeliDurakDto> duraklar)
    {
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
            .Where(x => x.Kaynak == "KocaeliBuyuksehir")
            .ToListAsync();

        var yeniDuraklar = new List<Durak>();

        foreach (var dto in duraklar)
        {
            if (string.IsNullOrWhiteSpace(dto.DurakKodu))
            {
                continue;
            }

            var mevcut = mevcutDuraklar.FirstOrDefault(x =>
                x.DurakKodu == dto.DurakKodu);

            if (mevcut != null)
            {
                mevcut.Ad = dto.Ad;
                mevcut.Enlem = dto.Enlem;
                mevcut.Boylam = dto.Boylam;
                mevcut.Aktif = dto.Aktif;

                continue;
            }

            var ilce = ilceler.FirstOrDefault(x =>
                x.Ad.Equals(
                    dto.Ilce,
                    StringComparison.OrdinalIgnoreCase));

            if (ilce == null)
            {
                continue;
            }

            yeniDuraklar.Add(new Durak
            {
                DurakKodu = dto.DurakKodu,
                Ad = dto.Ad,
                Kaynak = "KocaeliBuyuksehir",
                Enlem = dto.Enlem,
                Boylam = dto.Boylam,
                IlceId = ilce.Id,
                Aktif = dto.Aktif
            });
        }

        if (yeniDuraklar.Count > 0)
        {
            await _context.Duraklar.AddRangeAsync(
                yeniDuraklar);
        }

        await _context.SaveChangesAsync();

        return yeniDuraklar.Count;
    }
}

public class KocaeliDurakDto
{
    public string DurakKodu { get; set; } = string.Empty;

    public string Ad { get; set; } = string.Empty;

    public string Ilce { get; set; } = string.Empty;

    public double Enlem { get; set; }

    public double Boylam { get; set; }

    public bool Aktif { get; set; } = true;
}
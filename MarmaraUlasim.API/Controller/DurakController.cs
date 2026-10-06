using MarmaraUlasim.API.Data;
using MarmaraUlasim.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarmaraUlasim.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DuraklarController : ControllerBase
{
    private readonly MarmaraUlasimDbContext _context;
    private readonly KocaeliGtfsService _kocaeliGtfsService;

    public DuraklarController(
        MarmaraUlasimDbContext context,
        KocaeliGtfsService kocaeliGtfsService)
    {
        _context = context;
        _kocaeliGtfsService = kocaeliGtfsService;
    }

    // ============================================================
    // TÜM DURAKLAR
    // ============================================================

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var duraklar = await _context.Duraklar
            .Include(x => x.Ilce)
            .ThenInclude(x => x!.Il)
            .Include(x => x.Mahalle)
            .OrderBy(x => x.Ilce!.Ad)
            .ThenBy(x => x.Ad)
            .Select(x => new
            {
                x.Id,
                x.DurakKodu,
                x.Ad,
                x.Kaynak,
                x.Enlem,
                x.Boylam,
                x.Aktif,

                IlceId = x.IlceId,
                Ilce = x.Ilce!.Ad,

                MahalleId = x.MahalleId,
                Mahalle = x.Mahalle != null
                    ? x.Mahalle.Ad
                    : null,

                Il = x.Ilce!.Il!.Ad,
                PlakaKodu = x.Ilce!.Il!.PlakaKodu
            })
            .ToListAsync();

        return Ok(duraklar);
    }

    // ============================================================
    // ID İLE DURAK
    // ============================================================

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var durak = await _context.Duraklar
            .Include(x => x.Ilce)
            .ThenInclude(x => x!.Il)
            .Include(x => x.Mahalle)
            .Where(x => x.Id == id)
            .Select(x => new
            {
                x.Id,
                x.DurakKodu,
                x.Ad,
                x.Kaynak,
                x.Enlem,
                x.Boylam,
                x.Aktif,

                IlceId = x.IlceId,
                Ilce = x.Ilce!.Ad,

                MahalleId = x.MahalleId,
                Mahalle = x.Mahalle != null
                    ? x.Mahalle.Ad
                    : null,

                Il = x.Ilce!.Il!.Ad,
                PlakaKodu = x.Ilce!.Il!.PlakaKodu
            })
            .FirstOrDefaultAsync();

        if (durak == null)
        {
            return NotFound(new
            {
                message = "Durak bulunamadı."
            });
        }

        return Ok(durak);
    }

    // ============================================================
    // İLÇEYE GÖRE DURAKLAR
    // ============================================================

    [HttpGet("ilce/{ilceId:int}")]
    public async Task<IActionResult> GetByIlce(int ilceId)
    {
        var duraklar = await _context.Duraklar
            .Where(x => x.IlceId == ilceId)
            .Include(x => x.Mahalle)
            .OrderBy(x => x.Ad)
            .Select(x => new
            {
                x.Id,
                x.DurakKodu,
                x.Ad,
                x.Kaynak,
                x.Enlem,
                x.Boylam,
                x.Aktif,

                MahalleId = x.MahalleId,
                Mahalle = x.Mahalle != null
                    ? x.Mahalle.Ad
                    : null
            })
            .ToListAsync();

        return Ok(duraklar);
    }

    // ============================================================
    // MAHALLEYE GÖRE DURAKLAR
    // ============================================================

    [HttpGet("mahalle/{mahalleId:int}")]
    public async Task<IActionResult> GetByMahalle(int mahalleId)
    {
        var duraklar = await _context.Duraklar
            .Where(x => x.MahalleId == mahalleId)
            .OrderBy(x => x.Ad)
            .Select(x => new
            {
                x.Id,
                x.DurakKodu,
                x.Ad,
                x.Kaynak,
                x.Enlem,
                x.Boylam,
                x.Aktif,
                x.MahalleId
            })
            .ToListAsync();

        return Ok(duraklar);
    }

    // ============================================================
    // KOCAELİ GTFS AKTAR
    // ============================================================

    [HttpPost("import/kocaeli")]
    public async Task<IActionResult> ImportKocaeli(
        [FromQuery] string gtfsKlasoru)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(gtfsKlasoru))
            {
                return BadRequest(new
                {
                    message = "GTFS klasör yolu belirtilmelidir."
                });
            }

            var eklenen =
                await _kocaeliGtfsService
                    .DuraklariAktarAsync(gtfsKlasoru);

            return Ok(new
            {
                message = "Kocaeli durak aktarımı tamamlandı.",
                eklenenDurakSayisi = eklenen
            });
        }
        catch (FileNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message,
                dosya = ex.FileName
            });
        }

        catch (DirectoryNotFoundException ex)
{
    return NotFound(new
    {
        message = ex.Message
    });
}
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Kocaeli durak aktarımı sırasında hata oluştu.",
                detay = ex.Message
            });
        }
    }

    [HttpPost("import/kocaeli/hatlar")]
public async Task<IActionResult> ImportKocaeliHatlar(
    [FromQuery] string gtfsKlasoru)
{
    try
    {
        if (string.IsNullOrWhiteSpace(gtfsKlasoru))
        {
            return BadRequest(new
            {
                message = "GTFS klasör yolu belirtilmelidir."
            });
        }

        var eklenen =
            await _kocaeliGtfsService
                .HatlariAktarAsync(gtfsKlasoru);

        return Ok(new
        {
            message = "Kocaeli hat aktarımı tamamlandı.",
            eklenenHatSayisi = eklenen
        });
    }
    catch (DirectoryNotFoundException ex)
    {
        return NotFound(new
        {
            message = ex.Message
        });
    }
    catch (FileNotFoundException ex)
    {
        return NotFound(new
        {
            message = ex.Message,
            dosya = ex.FileName
        });
    }
    catch (Exception ex)
    {
        return StatusCode(500, new
        {
            message = "Kocaeli hat aktarımı sırasında hata oluştu.",
            detay = ex.Message
        });
    }
}

[HttpPost("import/kocaeli/seferler")]
public async Task<IActionResult> ImportKocaeliSeferler(
    [FromQuery] string gtfsKlasoru)
{
    try
    {
        if (string.IsNullOrWhiteSpace(gtfsKlasoru))
        {
            return BadRequest(new
            {
                message = "GTFS klasör yolu belirtilmelidir."
            });
        }

        var eklenen =
            await _kocaeliGtfsService
                .SeferleriAktarAsync(gtfsKlasoru);

        return Ok(new
        {
            message = "Kocaeli sefer aktarımı tamamlandı.",
            eklenenSeferSayisi = eklenen
        });
    }
    catch (DirectoryNotFoundException ex)
    {
        return NotFound(new
        {
            message = ex.Message
        });
    }
    catch (FileNotFoundException ex)
    {
        return NotFound(new
        {
            message = ex.Message,
            dosya = ex.FileName
        });
    }
    catch (Exception ex)
    {
        return StatusCode(500, new
        {
            message = "Kocaeli sefer aktarımı sırasında hata oluştu.",
            detay = ex.Message
        });
    }
}

    // ============================================================
    // DURAK SAYISI
    // ============================================================

    [HttpGet("sayisi")]
    public async Task<IActionResult> GetCount()
    {
        var sayi = await _context.Duraklar.CountAsync();

        return Ok(new
        {
            durakSayisi = sayi
        });
    }
}
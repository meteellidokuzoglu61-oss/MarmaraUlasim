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
    [HttpGet]
public async Task<IActionResult> GetAll(
    int sayfa = 1,
    int sayfaBoyutu = 100)
{
    if (sayfa < 1)
    {
        sayfa = 1;
    }

    if (sayfaBoyutu < 1)
    {
        sayfaBoyutu = 100;
    }

    if (sayfaBoyutu > 500)
    {
        sayfaBoyutu = 500;
    }

    var toplamSayi =
        await _context.Duraklar.CountAsync();

    var duraklar = await _context.Duraklar
        .AsNoTracking()
        .OrderBy(x => x.Ilce != null ? x.Ilce.Ad : "")
        .ThenBy(x => x.Ad)
        .Skip((sayfa - 1) * sayfaBoyutu)
        .Take(sayfaBoyutu)
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

            Ilce = x.Ilce != null
                ? x.Ilce.Ad
                : null,

            MahalleId = x.MahalleId,

            Mahalle = x.Mahalle != null
                ? x.Mahalle.Ad
                : null,

            Il = x.Ilce != null && x.Ilce.Il != null
                ? x.Ilce.Il.Ad
                : null,

            PlakaKodu =
                x.Ilce != null && x.Ilce.Il != null
                    ? (int?)x.Ilce.Il.PlakaKodu
                    : null
        })
        .ToListAsync();

    return Ok(new
    {
        sayfa,
        sayfaBoyutu,
        toplamSayi,
        toplamSayfa = (int)Math.Ceiling(
            toplamSayi / (double)sayfaBoyutu),
        veri = duraklar
    });
}


[HttpGet("seferler")]
public async Task<IActionResult> GetSeferler(
    int sayfa = 1,
    int sayfaBoyutu = 20)
{
    if (sayfa < 1)
        sayfa = 1;

    if (sayfaBoyutu < 1)
        sayfaBoyutu = 20;

    if (sayfaBoyutu > 100)
        sayfaBoyutu = 100;

    var seferler = await _context.Seferler
        .AsNoTracking()
        .Where(x => x.Kaynak == "KentKart-Kocaeli")
        .OrderBy(x => x.SeferKodu)
        .Skip((sayfa - 1) * sayfaBoyutu)
        .Take(sayfaBoyutu)
        .Select(x => new
        {
            x.Id,
            x.SeferKodu,
            x.HatKodu,
            x.ServisKodu,
            x.VarisYonu,
            x.ShapeId,
            x.Kaynak,
            x.Aktif
        })
        .ToListAsync();

    return Ok(seferler);
}
    // ============================================================
    // ID İLE DURAK
    // ============================================================

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var durak = await _context.Duraklar
            .AsNoTracking()
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

                Ilce = x.Ilce != null
                    ? x.Ilce.Ad
                    : null,

                MahalleId = x.MahalleId,

                Mahalle = x.Mahalle != null
                    ? x.Mahalle.Ad
                    : null,

                Il = x.Ilce != null && x.Ilce.Il != null
                    ? x.Ilce.Il.Ad
                    : null,

                PlakaKodu =
                    x.Ilce != null && x.Ilce.Il != null
                        ? (int?)x.Ilce.Il.PlakaKodu
                        : null
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
            .AsNoTracking()
            .Where(x => x.IlceId == ilceId)
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
            .AsNoTracking()
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
    // KOCAELİ GTFS - DURAK AKTAR
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
                message =
                    "Kocaeli durak aktarımı sırasında hata oluştu.",
                detay = ex.Message
            });
        }
    }

    // ============================================================
    // KOCAELİ GTFS - HATLAR
    // ============================================================

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
                message =
                    "Kocaeli hat aktarımı sırasında hata oluştu.",
                detay = ex.Message
            });
        }
    }

    // ============================================================
    // KOCAELİ GTFS - GÜZERGÂH
    // ============================================================

    [HttpPost("import/kocaeli/guzergah")]
    public async Task<IActionResult> ImportKocaeliGuzergah(
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
                    .GuzergahNoktalariniAktarAsync(
                        gtfsKlasoru);

            return Ok(new
            {
                message =
                    "Kocaeli güzergâh aktarımı tamamlandı.",

                eklenenGuzergahNoktasiSayisi =
                    eklenen
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
                message =
                    "Güzergâh aktarımı sırasında hata oluştu.",

                detay = ex.Message,

                innerException =
                    ex.InnerException?.Message,

                innerInnerException =
                    ex.InnerException?
                        .InnerException?
                        .Message
            });
        }
    }

    // ============================================================
    // KOCAELİ GTFS - SEFER SHAPE ID
    // ============================================================

    [HttpPost("import/kocaeli/sefer-shape-id")]
    public async Task<IActionResult> ImportKocaeliSeferShapeId(
        [FromQuery] string gtfsKlasoru)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(gtfsKlasoru))
            {
                return BadRequest(new
                {
                    message =
                        "GTFS klasör yolu belirtilmelidir."
                });
            }

            var guncellenen =
                await _kocaeliGtfsService
                    .SeferShapeIdleriniAktarAsync(
                        gtfsKlasoru);

            return Ok(new
            {
                message =
                    "Kocaeli sefer ShapeId aktarımı tamamlandı.",

                guncellenenSeferSayisi =
                    guncellenen
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
                message =
                    "Sefer ShapeId aktarımı sırasında hata oluştu.",

                detay = ex.Message,

                innerException =
                    ex.InnerException?.Message,

                innerInnerException =
                    ex.InnerException?
                        .InnerException?
                        .Message
            });
        }
    }

    // ============================================================
    // SEFER GÜZERGÂHI
    // ============================================================

    [HttpGet("sefer/{seferKodu}/guzergah")]
    public async Task<IActionResult> GetSeferGuzergahi(
        string seferKodu)
    {
        var sefer = await _context.Seferler
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.SeferKodu == seferKodu &&
                x.Kaynak == "KentKart-Kocaeli");

        if (sefer == null)
        {
            return NotFound(new
            {
                message = "Sefer bulunamadı."
            });
        }

        if (string.IsNullOrWhiteSpace(sefer.ShapeId))
        {
            return NotFound(new
            {
                message =
                    "Bu sefer için güzergâh bilgisi bulunamadı."
            });
        }

        var noktalar =
            await _context.GuzergahNoktalari
                .AsNoTracking()
                .Where(x =>
                    x.ShapeId == sefer.ShapeId &&
                    x.Kaynak == "KentKart-Kocaeli" &&
                    x.Aktif)
                .OrderBy(x => x.Sira)
                .Select(x => new
                {
                    x.Sira,
                    enlem = x.Enlem,
                    boylam = x.Boylam,
                    mesafe = x.Mesafe
                })
                .ToListAsync();

        return Ok(new
        {
            seferKodu = sefer.SeferKodu,
            hatKodu = sefer.HatKodu,
            shapeId = sefer.ShapeId,
            noktaSayisi = noktalar.Count,
            noktalar
        });
    }

    // ============================================================
    // KOCAELİ GTFS - SEFER DURAKLARI
    // ============================================================

    [HttpPost("import/kocaeli/sefer-duraklari")]
    public async Task<IActionResult> ImportKocaeliSeferDuraklari(
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
                    .SeferDuraklariniAktarAsync(
                        gtfsKlasoru);

            return Ok(new
            {
                message =
                    "Kocaeli sefer-durak aktarımı tamamlandı.",

                eklenenSeferDurakSayisi =
                    eklenen
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
                message =
                    "Sefer-durak aktarımı sırasında hata oluştu.",

                detay = ex.Message
            });
        }
    }

    // ============================================================
    // KOCAELİ GTFS - SEFERLER
    // ============================================================

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
                message =
                    "Kocaeli sefer aktarımı tamamlandı.",

                eklenenSeferSayisi =
                    eklenen
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
                message =
                    "Kocaeli sefer aktarımı sırasında hata oluştu.",

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
        var sayi =
            await _context.Duraklar.CountAsync();

        return Ok(new
        {
            durakSayisi = sayi
        });
    }
}
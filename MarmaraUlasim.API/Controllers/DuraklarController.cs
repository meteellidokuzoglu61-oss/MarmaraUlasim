using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarmaraUlasim.API.Data;

namespace MarmaraUlasim.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DuraklarController : ControllerBase
{
    private readonly MarmaraUlasimDbContext _context;

    public DuraklarController(MarmaraUlasimDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetDuraklar(
        [FromQuery] string? arama = null,
        [FromQuery] int? ilceId = null,
        [FromQuery] int sayfa = 1,
        [FromQuery] int sayfaBoyutu = 100)
    {
        sayfa = Math.Max(1, sayfa);
        sayfaBoyutu = Math.Clamp(sayfaBoyutu, 1, 500);

        var query = _context.Duraklar
            .AsNoTracking()
            .Where(x => x.Aktif);

        if (!string.IsNullOrWhiteSpace(arama))
        {
            arama = arama.Trim();
            query = query.Where(x =>
                x.Ad.Contains(arama) ||
                x.DurakKodu.Contains(arama));
        }

        if (ilceId.HasValue)
            query = query.Where(x => x.IlceId == ilceId.Value);

        var toplam = await query.CountAsync();

        var duraklar = await query
            .Include(x => x.Ilce)
                .ThenInclude(x => x!.Il)
            .Include(x => x.Mahalle)
            .OrderBy(x => x.Ad)
            .Skip((sayfa - 1) * sayfaBoyutu)
            .Take(sayfaBoyutu)
            .Select(x => new
            {
                id = x.Id,
                durakKodu = x.DurakKodu,
                ad = x.Ad,
                kaynak = x.Kaynak,
                enlem = x.Enlem,
                boylam = x.Boylam,
                aktif = x.Aktif,
                ilceId = x.IlceId,
                ilce = x.Ilce == null ? null : x.Ilce.Ad,
                mahalleId = x.MahalleId,
                mahalle = x.Mahalle == null ? null : x.Mahalle.Ad,
                il = x.Ilce == null || x.Ilce.Il == null ? null : x.Ilce.Il.Ad,
                plakaKodu = x.Ilce == null || x.Ilce.Il == null ? null : (int?)x.Ilce.Il.PlakaKodu
            })
            .ToListAsync();

        return Ok(new
        {
            toplam,
            sayfa,
            sayfaBoyutu,
            veriler = duraklar
        });
    }

    [HttpGet("seferler")]
    public async Task<IActionResult> GetSeferler(
        [FromQuery] int sayfa = 1,
        [FromQuery] int sayfaBoyutu = 50)
    {
        sayfa = Math.Max(1, sayfa);
        sayfaBoyutu = Math.Clamp(sayfaBoyutu, 1, 200);

        var seferler = await _context.Seferler
            .AsNoTracking()
            .Where(x => x.Aktif)
            .OrderBy(x => x.HatKodu)
            .ThenBy(x => x.SeferKodu)
            .Skip((sayfa - 1) * sayfaBoyutu)
            .Take(sayfaBoyutu)
            .Select(x => new
            {
                x.Id,
                seferKodu = x.SeferKodu,
                hatKodu = x.HatKodu,
                servisKodu = x.ServisKodu,
                varisYonu = x.VarisYonu,
                shapeId = x.ShapeId,
                kaynak = x.Kaynak,
                aktif = x.Aktif
            })
            .ToListAsync();

        return Ok(seferler);
    }

    [HttpGet("sefer/{seferKodu}/guzergah")]
    public async Task<IActionResult> GetSeferGuzergahi(string seferKodu)
    {
        var sefer = await _context.Seferler
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.SeferKodu == seferKodu && x.Aktif);

        if (sefer == null)
            return NotFound(new { mesaj = "Sefer bulunamadı." });

        if (string.IsNullOrWhiteSpace(sefer.ShapeId))
            return Ok(new
            {
                seferKodu = sefer.SeferKodu,
                hatKodu = sefer.HatKodu,
                shapeId = "",
                noktaSayisi = 0,
                noktalar = Array.Empty<object>()
            });

        var noktalar = await _context.GuzergahNoktalari
            .AsNoTracking()
            .Where(x =>
                x.Aktif &&
                x.ShapeId == sefer.ShapeId &&
                x.Kaynak == sefer.Kaynak)
            .OrderBy(x => x.Sira)
            .Select(x => new
            {
                sira = x.Sira,
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
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarmaraUlasim.API.Data;

namespace MarmaraUlasim.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IllerController : ControllerBase
{
    private readonly MarmaraUlasimDbContext _context;

    public IllerController(MarmaraUlasimDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetIller()
    {
        var iller = await _context.Iller
            .AsNoTracking()
            .Select(x => new
            {
                id = x.Id,
                ad = x.Ad,
                plakaKodu = x.PlakaKodu,
                ilceSayisi = _context.Ilceler.Count(i => i.IlId == x.Id),
                durakSayisi = _context.Duraklar.Count(d => d.Ilce != null && d.Ilce.IlId == x.Id && d.Aktif)
            })
            .OrderBy(x => x.ad)
            .ToListAsync();

        return Ok(iller);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetIl(int id)
    {
        var il = await _context.Iller
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new
            {
                id = x.Id,
                ad = x.Ad,
                plakaKodu = x.PlakaKodu,
                ilceler = _context.Ilceler
                    .Where(i => i.IlId == x.Id)
                    .OrderBy(i => i.Ad)
                    .Select(i => new
                    {
                        id = i.Id,
                        apiId = i.ApiId,
                        ad = i.Ad,
                        mahalleSayisi = _context.Mahalleler.Count(m => m.IlceId == i.Id),
                        durakSayisi = _context.Duraklar.Count(d => d.IlceId == i.Id && d.Aktif)
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();

        if (il == null)
            return NotFound(new { mesaj = "İl bulunamadı." });

        return Ok(il);
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarmaraUlasim.API.Data;

namespace MarmaraUlasim.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IlcelerController : ControllerBase
{
    private readonly MarmaraUlasimDbContext _context;

    public IlcelerController(MarmaraUlasimDbContext context)
    {
        _context = context;
    }

    [HttpGet("il/{ilId:int}")]
    public async Task<IActionResult> GetIlceler(int ilId)
    {
        var ilceler = await _context.Ilceler
            .AsNoTracking()
            .Where(x => x.IlId == ilId)
            .OrderBy(x => x.Ad)
            .Select(x => new
            {
                id = x.Id,
                apiId = x.ApiId,
                ad = x.Ad,
                mahalleSayisi = _context.Mahalleler.Count(m => m.IlceId == x.Id),
                durakSayisi = _context.Duraklar.Count(d => d.IlceId == x.Id && d.Aktif)
            })
            .ToListAsync();

        return Ok(ilceler);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetIlce(int id)
    {
        var ilce = await _context.Ilceler
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new
            {
                id = x.Id,
                ad = x.Ad,
                ilId = x.IlId,
                il = x.Il == null ? null : x.Il.Ad,
                mahalleler = _context.Mahalleler
                    .Where(m => m.IlceId == x.Id)
                    .OrderBy(m => m.Ad)
                    .Select(m => new { id = m.Id, ad = m.Ad })
                    .ToList()
            })
            .FirstOrDefaultAsync();

        if (ilce == null)
            return NotFound(new { mesaj = "İlçe bulunamadı." });

        return Ok(ilce);
    }
}

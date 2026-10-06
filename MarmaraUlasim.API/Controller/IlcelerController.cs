using MarmaraUlasim.API.Data;
using MarmaraUlasim.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarmaraUlasim.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IlcelerController : ControllerBase
{
    private readonly MarmaraUlasimDbContext _context;
    private readonly TurkiyeApiService _turkiyeApiService;

    public IlcelerController(
        MarmaraUlasimDbContext context,
        TurkiyeApiService turkiyeApiService)
    {
        _context = context;
        _turkiyeApiService = turkiyeApiService;
    }

    // Tüm ilçeleri getir
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var ilceler = await _context.Ilceler
            .Include(x => x.Il)
            .OrderBy(x => x.Il!.Ad)
            .ThenBy(x => x.Ad)
            .Select(x => new
            {
                x.Id,
                x.ApiId,
                x.Ad,
                IlId = x.IlId,
                Il = x.Il!.Ad,
                PlakaKodu = x.Il!.PlakaKodu
            })
            .ToListAsync();

        return Ok(ilceler);
    }

    // Belirli bir ilin ilçelerini getir
    [HttpGet("il/{ilId:int}")]
    public async Task<IActionResult> GetByIl(int ilId)
    {
        var ilceler = await _context.Ilceler
            .Where(x => x.IlId == ilId)
            .OrderBy(x => x.Ad)
            .Select(x => new
            {
                x.Id,
                x.ApiId,
                x.Ad,
                x.IlId
            })
            .ToListAsync();

        return Ok(ilceler);
    }

    // TurkiyeAPI'den ilçeleri çekip PostgreSQL'e aktar
    [HttpPost("import")]
    public async Task<IActionResult> Import()
    {
        try
        {
            var eklenen = await _turkiyeApiService.IlceleriAktarAsync();

            return Ok(new
            {
                message = "İlçe aktarımı tamamlandı.",
                eklenenIlceSayisi = eklenen
            });
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(502, new
            {
                message = "TurkiyeAPI'ye ulaşılamadı.",
                detay = ex.Message
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "İlçe aktarımı sırasında hata oluştu.",
                detay = ex.Message
            });
        }
    }
}
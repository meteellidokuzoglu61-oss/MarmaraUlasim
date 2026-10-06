using MarmaraUlasim.API.Data;
using MarmaraUlasim.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarmaraUlasim.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MahallelerController : ControllerBase
{
    private readonly MarmaraUlasimDbContext _context;
    private readonly TurkiyeApiService _turkiyeApiService;

    public MahallelerController(
        MarmaraUlasimDbContext context,
        TurkiyeApiService turkiyeApiService)
    {
        _context = context;
        _turkiyeApiService = turkiyeApiService;
    }


    // ============================================================
    // TÜM MAHALLELERİ GETİR
    // ============================================================

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var mahalleler = await _context.Mahalleler
            .Include(x => x.Ilce)
            .ThenInclude(x => x!.Il)
            .OrderBy(x => x.Ilce!.Il!.Ad)
            .ThenBy(x => x.Ilce!.Ad)
            .ThenBy(x => x.Ad)
            .Select(x => new
            {
                x.Id,
                x.ApiId,
                x.Ad,

                IlceId = x.IlceId,
                Ilce = x.Ilce!.Ad,

                IlId = x.Ilce!.IlId,
                Il = x.Ilce!.Il!.Ad,

                PlakaKodu = x.Ilce!.Il!.PlakaKodu
            })
            .ToListAsync();

        return Ok(mahalleler);
    }


    // ============================================================
    // BELİRLİ BİR İLÇENİN MAHALLELERİNİ GETİR
    // ============================================================

    [HttpGet("ilce/{ilceId:int}")]
    public async Task<IActionResult> GetByIlce(int ilceId)
    {
        var mahalleler = await _context.Mahalleler
            .Where(x => x.IlceId == ilceId)
            .OrderBy(x => x.Ad)
            .Select(x => new
            {
                x.Id,
                x.ApiId,
                x.Ad,
                x.IlceId
            })
            .ToListAsync();

        return Ok(mahalleler);
    }


    // ============================================================
    // TURKIYE API'DEN MAHALLELERİ AKTAR
    // ============================================================

    [HttpPost("import")]
    public async Task<IActionResult> Import()
    {
        try
        {
            var eklenen =
                await _turkiyeApiService.MahalleleriAktarAsync();

            return Ok(new
            {
                message = "Marmara Bölgesi mahalle aktarımı tamamlandı.",
                eklenenMahalleSayisi = eklenen
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
                message = "Mahalle aktarımı sırasında hata oluştu.",
                detay = ex.Message
            });
        }
    }
}
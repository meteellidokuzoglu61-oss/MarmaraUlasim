using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarmaraUlasim.API.Data;

namespace MarmaraUlasim.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MahallelerController : ControllerBase
{
    private readonly MarmaraUlasimDbContext _context;

    public MahallelerController(MarmaraUlasimDbContext context)
    {
        _context = context;
    }

    [HttpGet("ilce/{ilceId:int}")]
    public async Task<IActionResult> GetMahalleler(int ilceId)
    {
        var mahalleler = await _context.Mahalleler
            .AsNoTracking()
            .Where(x => x.IlceId == ilceId)
            .OrderBy(x => x.Ad)
            .Select(x => new
            {
                id = x.Id,
                apiId = x.ApiId,
                ad = x.Ad
            })
            .ToListAsync();

        return Ok(mahalleler);
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarmaraUlasim.API.Data;
using MarmaraUlasim.API.Models;

namespace MarmaraUlasim.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HatlarController : ControllerBase
{
    private readonly MarmaraUlasimDbContext _context;

    public HatlarController(MarmaraUlasimDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Hat>>> GetHatlar()
    {
        var hatlar = await _context.Hatlar
            .AsNoTracking()
            .Where(x => x.Aktif)
            .OrderBy(x => x.HatKodu)
            .ToListAsync();

        return Ok(hatlar);
    }
}
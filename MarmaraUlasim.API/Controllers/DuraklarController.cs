using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarmaraUlasim.API.Data;
using MarmaraUlasim.API.Models;

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
    public async Task<ActionResult<IEnumerable<Durak>>> GetDuraklar()
    {
        var duraklar = await _context.Duraklar
            .AsNoTracking()
            .Where(x => x.Aktif)
            .Include(x => x.Ilce)
            .Include(x => x.Mahalle)
            .OrderBy(x => x.Ad)
            .ToListAsync();

        return Ok(duraklar);
    }
}
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
    public async Task<IActionResult> GetAll()
    {
        var iller = await _context.Iller
            .OrderBy(x => x.PlakaKodu)
            .ToListAsync();

        return Ok(iller);
    }
}
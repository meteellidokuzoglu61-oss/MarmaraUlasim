using Microsoft.AspNetCore.Mvc;
using MarmaraUlasim.API.Services;

namespace MarmaraUlasim.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GtfsController : ControllerBase
{
    private readonly KocaeliGtfsService _gtfs;

    public GtfsController(KocaeliGtfsService gtfs)
    {
        _gtfs = gtfs;
    }

    [HttpPost("kocaeli-aktar")]
    public async Task<IActionResult> KocaeliAktar([FromQuery] string klasor)
    {
        if (string.IsNullOrWhiteSpace(klasor))
            return BadRequest(new { mesaj = "GTFS klasörü belirtilmelidir." });

        var durak = await _gtfs.DuraklariAktarAsync(klasor);
        var hat = await _gtfs.HatlariAktarAsync(klasor);
        var sefer = await _gtfs.SeferleriAktarAsync(klasor);
        var seferDurak = await _gtfs.SeferDuraklariniAktarAsync(klasor);
        var guzergah = await _gtfs.GuzergahNoktalariniAktarAsync(klasor);
        var shape = await _gtfs.SeferShapeIdleriniAktarAsync(klasor);

        return Ok(new
        {
            mesaj = "Kocaeli GTFS aktarımı tamamlandı.",
            durak,
            hat,
            sefer,
            seferDurak,
            guzergah,
            shape
        });
    }
}

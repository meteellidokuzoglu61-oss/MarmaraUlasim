using Microsoft.AspNetCore.Mvc;
using MarmaraUlasim.API.Services;

namespace MarmaraUlasim.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VeriController : ControllerBase
{
    private readonly TurkiyeApiService _turkiyeApi;

    public VeriController(TurkiyeApiService turkiyeApi)
    {
        _turkiyeApi = turkiyeApi;
    }

    [HttpPost("ilceleri-aktar")]
    public async Task<IActionResult> IlceleriAktar()
    {
        var adet = await _turkiyeApi.IlceleriAktarAsync();
        return Ok(new { mesaj = "İlçeler aktarıldı.", adet });
    }

    [HttpPost("mahalleleri-aktar")]
    public async Task<IActionResult> MahalleleriAktar()
    {
        var adet = await _turkiyeApi.MahalleleriAktarAsync();
        return Ok(new { mesaj = "Mahalleler aktarıldı.", adet });
    }
}

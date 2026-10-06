using System.Net.Http.Json;
using MarmaraUlasim.API.Data;
using MarmaraUlasim.API.Models;
using Microsoft.EntityFrameworkCore;

namespace MarmaraUlasim.API.Services;

public class TurkiyeApiService
{
    private const string ApiUrl =
        "https://api.turkiyeapi.dev/v1/districts";

    private readonly HttpClient _httpClient;
    private readonly MarmaraUlasimDbContext _context;

    public TurkiyeApiService(
        HttpClient httpClient,
        MarmaraUlasimDbContext context)
    {
        _httpClient = httpClient;
        _context = context;
    }

   public async Task<int> IlceleriAktarAsync()
{
    var response =
        await _httpClient.GetFromJsonAsync<TurkiyeApiResponse>(ApiUrl);

    if (response == null || response.Data == null)
    {
        throw new Exception("TurkiyeAPI'den veri alınamadı.");
    }

    var marmaraPlakalari = new HashSet<int>
    {
        10, 11, 16, 17, 22, 34, 39, 41, 54, 59, 77
    };

    var toplamKayit = response.Data.Count;

    var marmaraKayitlari = response.Data
        .Where(x => marmaraPlakalari.Contains(x.ProvinceId))
        .ToList();

    var iller = await _context.Iller
        .AsNoTracking()
        .ToListAsync();

    var ilByPlate = iller.ToDictionary(
        x => x.PlakaKodu,
        x => x
    );

    var eslesenKayitlar = marmaraKayitlari
        .Where(x => ilByPlate.ContainsKey(x.ProvinceId))
        .ToList();

    var mevcutApiIdler = await _context.Ilceler
        .Select(x => x.ApiId)
        .ToHashSetAsync();

    var yeniIlceler = new List<Ilce>();

    foreach (var dto in eslesenKayitlar)
    {
        if (mevcutApiIdler.Contains(dto.Id))
        {
            continue;
        }

        var il = ilByPlate[dto.ProvinceId];

        yeniIlceler.Add(new Ilce
        {
            ApiId = dto.Id,
            Ad = dto.Name,
            IlId = il.Id
        });

        mevcutApiIdler.Add(dto.Id);
    }

    if (yeniIlceler.Count > 0)
    {
        await _context.Ilceler.AddRangeAsync(yeniIlceler);
        await _context.SaveChangesAsync();
    }

    throw new Exception(
        $"Toplam API kaydı: {toplamKayit} | " +
        $"Marmara kaydı: {marmaraKayitlari.Count} | " +
        $"İl ile eşleşen kayıt: {eslesenKayitlar.Count} | " +
        $"Yeni ilçe: {yeniIlceler.Count}"
    );
}
}


// TurkiyeAPI ana cevap modeli
public class TurkiyeApiResponse
{
    public string Status { get; set; } = string.Empty;

    public List<TurkiyeApiIlceDto> Data { get; set; } = new();
}


// TurkiyeAPI ilçe modeli
public class TurkiyeApiIlceDto
{
    public int ProvinceId { get; set; }

    public int Id { get; set; }

    public string Province { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
}
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

    var ilkKayitlar = response.Data
        .Take(10)
        .Select(x => new
        {
            x.ProvinceId,
            x.Province,
            x.Id,
            x.Name
        })
        .ToList();

    throw new Exception(
        System.Text.Json.JsonSerializer.Serialize(ilkKayitlar)
    );
}

    // Türkçe karakter ve büyük/küçük harf farklarını azaltır
    private static string Normalize(string value)
    {
        return value
            .Trim()
            .ToUpperInvariant()
            .Replace("İ", "I")
            .Replace("Ş", "S")
            .Replace("Ğ", "G")
            .Replace("Ü", "U")
            .Replace("Ö", "O")
            .Replace("Ç", "C");
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
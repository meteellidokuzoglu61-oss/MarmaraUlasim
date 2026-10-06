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
        // TurkiyeAPI'den bütün ilçeleri çek
        var response =
            await _httpClient.GetFromJsonAsync<TurkiyeApiResponse>(ApiUrl);

        if (response == null || response.Data == null)
        {
            throw new Exception(
                "TurkiyeAPI'den ilçe verileri alınamadı.");
        }

        // PostgreSQL'deki illeri al
        var iller = await _context.Iller
            .AsNoTracking()
            .ToListAsync();

        // Plaka koduna göre hızlı arama
        var ilByPlate = iller.ToDictionary(
            x => x.PlakaKodu,
            x => x
        );

        // Daha önce aktarılmış ilçe API ID'lerini al
        var mevcutApiIdler = await _context.Ilceler
            .Select(x => x.ApiId)
            .ToHashSetAsync();

        var yeniIlceler = new List<Ilce>();

        foreach (var dto in response.Data)
        {
            /*
             * TurkiyeAPI provinceId değerini
             * bizim PlakaKodu ile karşılaştırıyoruz.
             *
             * Örneğin:
             * İstanbul = 34
             * Kocaeli = 41
             * Sakarya = 54
             *
             * Adana = 1 olduğu için
             * bizim Marmara illerimiz arasında bulunamayacak.
             */

            if (!ilByPlate.TryGetValue(
                    dto.ProvinceId,
                    out var il))
            {
                continue;
            }

            // Daha önce aktarılmışsa tekrar ekleme
            if (mevcutApiIdler.Contains(dto.Id))
            {
                continue;
            }

            yeniIlceler.Add(new Ilce
            {
                ApiId = dto.Id,
                Ad = dto.Name,
                IlId = il.Id
            });

            mevcutApiIdler.Add(dto.Id);
        }

        // Yeni ilçeleri PostgreSQL'e kaydet
        if (yeniIlceler.Count > 0)
        {
            await _context.Ilceler.AddRangeAsync(yeniIlceler);

            await _context.SaveChangesAsync();
        }

        return yeniIlceler.Count;
    }
}


// TurkiyeAPI ana cevabı
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
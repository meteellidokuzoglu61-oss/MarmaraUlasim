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

    // ============================================================
    // İLÇELERİ AKTAR
    // ============================================================

    public async Task<int> IlceleriAktarAsync()
    {
        var response =
            await _httpClient.GetFromJsonAsync<TurkiyeApiResponse>(ApiUrl);

        if (response == null || response.Data == null)
        {
            throw new Exception(
                "TurkiyeAPI'den ilçe verileri alınamadı.");
        }

        var marmaraPlakalari = new HashSet<int>
        {
            10, // Balıkesir
            11, // Bilecik
            16, // Bursa
            17, // Çanakkale
            22, // Edirne
            34, // İstanbul
            39, // Kırklareli
            41, // Kocaeli
            54, // Sakarya
            59, // Tekirdağ
            77  // Yalova
        };

        var iller = await _context.Iller
            .AsNoTracking()
            .ToListAsync();

        var ilByPlate = iller.ToDictionary(
            x => x.PlakaKodu,
            x => x
        );

        var mevcutApiIdler = await _context.Ilceler
            .Select(x => x.ApiId)
            .ToHashSetAsync();

        var yeniIlceler = new List<Ilce>();

        foreach (var dto in response.Data)
        {
            if (!marmaraPlakalari.Contains(dto.ProvinceId))
            {
                continue;
            }

            if (!ilByPlate.TryGetValue(
                    dto.ProvinceId,
                    out var il))
            {
                continue;
            }

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

        if (yeniIlceler.Count > 0)
        {
            await _context.Ilceler.AddRangeAsync(yeniIlceler);
            await _context.SaveChangesAsync();
        }

        return yeniIlceler.Count;
    }


    // ============================================================
    // MAHALLELERİ AKTAR
    // ============================================================

    public async Task<int> MahalleleriAktarAsync()
    {
        // TurkiyeAPI'den ilçeleri ve içlerindeki mahalleleri çek
        var response =
            await _httpClient.GetFromJsonAsync<TurkiyeApiResponse>(ApiUrl);

        if (response == null || response.Data == null)
        {
            throw new Exception(
                "TurkiyeAPI'den mahalle verileri alınamadı.");
        }

        // Marmara Bölgesi illeri
        var marmaraPlakalari = new HashSet<int>
        {
            10, // Balıkesir
            11, // Bilecik
            16, // Bursa
            17, // Çanakkale
            22, // Edirne
            34, // İstanbul
            39, // Kırklareli
            41, // Kocaeli
            54, // Sakarya
            59, // Tekirdağ
            77  // Yalova
        };

        // PostgreSQL'deki ilçeleri getir
        var ilceler = await _context.Ilceler
            .AsNoTracking()
            .ToListAsync();

        // TurkiyeAPI ilçe ID'si → bizim İlçe ID'miz
        var ilceByApiId = ilceler.ToDictionary(
            x => x.ApiId,
            x => x
        );

        // Daha önce aktarılmış mahalle API ID'lerini getir
        var mevcutApiIdler = await _context.Mahalleler
            .Select(x => x.ApiId)
            .ToHashSetAsync();

        var yeniMahalleler = new List<Mahalle>();

        // TurkiyeAPI'den gelen ilçeleri dolaş
        foreach (var ilceDto in response.Data)
        {
            // Marmara dışındaki illeri atla
            if (!marmaraPlakalari.Contains(ilceDto.ProvinceId))
            {
                continue;
            }

            // PostgreSQL'de bu ilçenin karşılığı var mı?
            if (!ilceByApiId.TryGetValue(
                    ilceDto.Id,
                    out var ilce))
            {
                continue;
            }

            // İlçenin mahalleleri
            foreach (var mahalleDto in ilceDto.Neighborhoods)
            {
                // Mahalle zaten varsa tekrar ekleme
                if (mevcutApiIdler.Contains(mahalleDto.Id))
                {
                    continue;
                }

                yeniMahalleler.Add(new Mahalle
                {
                    ApiId = mahalleDto.Id,
                    Ad = mahalleDto.Name,
                    IlceId = ilce.Id
                });

                // Aynı çalıştırmada tekrar eklenmesini önle
                mevcutApiIdler.Add(mahalleDto.Id);
            }
        }

        // Yeni mahalleleri PostgreSQL'e kaydet
        if (yeniMahalleler.Count > 0)
        {
            await _context.Mahalleler.AddRangeAsync(yeniMahalleler);

            await _context.SaveChangesAsync();
        }

        return yeniMahalleler.Count;
    }
}


// ============================================================
// TURKIYE API ANA CEVAP
// ============================================================

public class TurkiyeApiResponse
{
    public string Status { get; set; } = string.Empty;

    public List<TurkiyeApiIlceDto> Data { get; set; } = new();
}


// ============================================================
// TURKIYE API İLÇE
// ============================================================

public class TurkiyeApiIlceDto
{
    public int ProvinceId { get; set; }

    public int Id { get; set; }

    public string Province { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    // İlçenin mahalleleri
    public List<TurkiyeApiMahalleDto> Neighborhoods { get; set; } = new();
}


// ============================================================
// TURKIYE API MAHALLE
// ============================================================

public class TurkiyeApiMahalleDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int Population { get; set; }
}
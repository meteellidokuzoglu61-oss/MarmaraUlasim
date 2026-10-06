namespace MarmaraUlasim.API.Models;

public class Hat
{
    public int Id { get; set; }

    // GTFS route_id
    public string HatKodu { get; set; } = string.Empty;

    // Hat adı / kısa adı
    public string Ad { get; set; } = string.Empty;

    // Örn: Bus
    public string? Tip { get; set; }

    // Hat açıklaması
    public string? Aciklama { get; set; }

    // Veri kaynağı
    public string Kaynak { get; set; } = string.Empty;

    public bool Aktif { get; set; } = true;
}
namespace MarmaraUlasim.API.Models;

public class Sefer
{
    public int Id { get; set; }

    public string SeferKodu { get; set; } = string.Empty;

    public string HatKodu { get; set; } = string.Empty;

    public string? ServisKodu { get; set; }

    public string? VarisYonu { get; set; }

    public string Kaynak { get; set; } = string.Empty;

    public bool Aktif { get; set; } = true;

    public Hat? Hat { get; set; }
}
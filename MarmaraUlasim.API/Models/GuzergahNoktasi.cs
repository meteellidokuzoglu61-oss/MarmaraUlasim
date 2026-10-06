namespace MarmaraUlasim.API.Models;

public class GuzergahNoktasi
{
    public int Id { get; set; }

    public string ShapeId { get; set; } = string.Empty;

    public double Enlem { get; set; }

    public double Boylam { get; set; }

    public int Sira { get; set; }

    public double? Mesafe { get; set; }

    public string Kaynak { get; set; } = string.Empty;

    public bool Aktif { get; set; } = true;
}
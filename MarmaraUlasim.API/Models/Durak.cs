namespace MarmaraUlasim.API.Models;

public class Durak
{
    public int Id { get; set; }

    public string DurakKodu { get; set; } = string.Empty;

    public string Ad { get; set; } = string.Empty;

    public string Kaynak { get; set; } = string.Empty;

    public double Enlem { get; set; }

    public double Boylam { get; set; }

    public int? IlceId { get; set; }

    public Ilce? Ilce { get; set; }

    public int? MahalleId { get; set; }

    public Mahalle? Mahalle { get; set; }

    public bool Aktif { get; set; } = true;
}
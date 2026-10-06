namespace MarmaraUlasim.API.Models;

public class SeferDurak
{
    public int Id { get; set; }

    public string SeferKodu { get; set; } = string.Empty;

    public string DurakKodu { get; set; } = string.Empty;

    public int DurakSirasi { get; set; }

    public TimeSpan? VarisSaati { get; set; }

    public TimeSpan? KalkisSaati { get; set; }

    public bool Aktif { get; set; } = true;

    public Sefer? Sefer { get; set; }

    public Durak? Durak { get; set; }
}
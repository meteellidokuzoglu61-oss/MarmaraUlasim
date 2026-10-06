namespace MarmaraUlasim.API.Models;

public class Mahalle
{
    public int Id { get; set; }

    // TurkiyeAPI'deki mahalle ID'si
    public int ApiId { get; set; }

    // Mahalle adı
    public string Ad { get; set; } = string.Empty;

    // Bizim PostgreSQL'deki İlçe ID'miz
    public int IlceId { get; set; }

    // İlçe ilişkisi
    public Ilce? Ilce { get; set; }
}
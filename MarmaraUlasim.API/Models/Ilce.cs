namespace MarmaraUlasim.API.Models;

public class Ilce
{
    public int Id { get; set; }

    // TurkiyeAPI'deki ilçe ID'si
    public int ApiId { get; set; }

    public string Ad { get; set; } = string.Empty;

    // Bizim PostgreSQL'deki Il ID'miz
    public int IlId { get; set; }

    public Il? Il { get; set; }
}
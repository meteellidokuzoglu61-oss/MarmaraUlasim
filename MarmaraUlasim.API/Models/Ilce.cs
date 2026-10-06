namespace MarmaraUlasim.API.Models;

public class Ilce
{
    public int Id { get; set; }

    public string Ad { get; set; } = string.Empty;

    public int IlId { get; set; }

    public Il? Il { get; set; }
}
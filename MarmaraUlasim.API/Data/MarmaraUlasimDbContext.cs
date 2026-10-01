using Microsoft.EntityFrameworkCore;
using MarmaraUlasim.API.Models;

namespace MarmaraUlasim.API.Data;

public class MarmaraUlasimDbContext : DbContext
{
    public MarmaraUlasimDbContext(
        DbContextOptions<MarmaraUlasimDbContext> options)
        : base(options)
    {
    }

    public DbSet<Il> Iller => Set<Il>();
}
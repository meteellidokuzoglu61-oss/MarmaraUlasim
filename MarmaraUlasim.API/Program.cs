using Microsoft.EntityFrameworkCore;
using MarmaraUlasim.API.Data;
using MarmaraUlasim.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddScoped<TurkiyeApiService>();
builder.Services.AddScoped<KocaeliGtfsService>();
builder.Services.AddHostedService<MarmaraStartupSeeder>();

// PostgreSQL
builder.Services.AddDbContext<MarmaraUlasimDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("Default")
    )
);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("Ionic", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger'ı her ortamda aç
app.UseSwagger();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "MarmaraUlasim API V1");
    options.RoutePrefix = "swagger";
});

// CORS
app.UseCors("Ionic");

// HTTPS yönlendirmesini şimdilik kapatıyoruz
// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
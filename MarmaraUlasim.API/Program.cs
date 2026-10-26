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
builder.Services.AddHostedService<KocaeliStartupImporter>();

// PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("Default");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Veritabanı bağlantısı tanımlı değil. ConnectionStrings__Default ortam değişkenini ayarlayın."
    );
}

builder.Services.AddDbContext<MarmaraUlasimDbContext>(options =>
    options.UseNpgsql(connectionString)
);

// CORS: yalnızca yapılandırmada açıkça izin verilen kaynaklar.
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? Array.Empty<string>();

if (allowedOrigins.Length == 0)
{
    throw new InvalidOperationException(
        "CORS kaynakları tanımlı değil. Cors:AllowedOrigins ayarını yapılandırın."
    );
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("Ionic", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger yalnızca geliştirme ortamında açılır.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "MarmaraUlasim API V1");
        options.RoutePrefix = "swagger";
    });
}

// CORS
app.UseCors("Ionic");

// HTTPS yönlendirmesini şimdilik kapatıyoruz
// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
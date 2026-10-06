using Microsoft.EntityFrameworkCore;
using MarmaraUlasim.API.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<MarmaraUlasimDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("Default")
    )
);

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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Ionic");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
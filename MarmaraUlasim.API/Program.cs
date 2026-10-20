using MarmaraUlasim.API.Data;
using MarmaraUlasim.API.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "ConnectionStrings:Default is missing. Configure it with .NET User Secrets locally or the ConnectionStrings__Default environment variable in production.");
}

builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddScoped<TurkiyeApiService>();
builder.Services.AddScoped<KocaeliGtfsService>();
builder.Services.AddHostedService<MarmaraStartupSeeder>();
builder.Services.AddHostedService<KocaeliStartupImporter>();
builder.Services.AddDbContext<MarmaraUlasimDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddHealthChecks();

var configuredOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Ionic", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            policy
                .WithOrigins(
                    "http://localhost:4200",
                    "http://localhost:8100",
                    "http://127.0.0.1:4200",
                    "http://127.0.0.1:8100",
                    "capacitor://localhost",
                    "ionic://localhost")
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
        else if (configuredOrigins.Length > 0)
        {
            policy
                .WithOrigins(configuredOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
    });
});

var adminApiKey = builder.Configuration["Admin:ApiKey"];

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("DataImport", policy =>
    {
        policy.RequireAssertion(context =>
        {
            if (builder.Environment.IsDevelopment())
            {
                return true;
            }

            if (string.IsNullOrWhiteSpace(adminApiKey) ||
                context.Resource is not HttpContext httpContext)
            {
                return false;
            }

            var suppliedKey = httpContext.Request.Headers["X-Admin-Key"].ToString();
            return !string.IsNullOrWhiteSpace(suppliedKey) &&
                   string.Equals(suppliedKey, adminApiKey, StringComparison.Ordinal);
        });
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Apply checked-in migrations before the background data importers start.
await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MarmaraUlasimDbContext>();
    await db.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "MarmaraUlasim API V1");
        options.RoutePrefix = "swagger";
    });
}
else
{
    app.UseHttpsRedirection();
}

app.UseCors("Ionic");
app.UseAuthorization();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();
app.MapHealthChecks("/health");
app.MapFallbackToFile("index.html");

app.Run();

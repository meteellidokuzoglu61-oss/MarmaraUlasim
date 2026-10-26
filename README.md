# MarmaraUlasim

Marmara Bölgesi'ndeki iller, ilçeler, mahalleler, duraklar, hatlar ve güzergâh verilerini API üzerinden sunmayı hedefleyen Ionic/Angular + ASP.NET Core + PostgreSQL projesi.

## Teknolojiler

- Ionic + Angular (standalone components)
- ASP.NET Core Web API (.NET 10)
- Entity Framework Core + PostgreSQL
- Kocaeli GTFS verileri ve Leaflet haritası

## Geliştirme ortamını başlatma

### 1. Veritabanı bağlantısı

Bağlantı bilgileri kaynak koduna yazılmaz. PowerShell'de API'yi başlatmadan önce kendi yerel PostgreSQL bilgilerini ayarla:

```powershell
$env:ConnectionStrings__Default = "Host=localhost;Port=5432;Database=MarmaraUlasim;Username=postgres;Password=KENDI_YEREL_PAROLAN"
dotnet run --project .\MarmaraUlasim.API
```

Parolanı bu dosyaya veya GitHub'a ekleme. Yerel terminal oturumu kapandığında ortam değişkeni tekrar ayarlanmalıdır.

### 2. Ionic/Angular

```powershell
npm install
npm start
```

Geliştirme ortamı API adresi `http://localhost:5197/api` olarak ayarlanmıştır. Üretim derlemesinde `src/environments/environment.prod.ts` içindeki `apiUrl` değerini dağıtım mimarine göre ayarla. API ayrı bir alan adında çalışacaksa CORS izin listesine gerçek web uygulaması kaynağını da ekle.

### 3. API dokümantasyonu

Swagger yalnızca ASP.NET Core Development ortamında açılır. Üretim ortamında Swagger arayüzü otomatik olarak sunulmaz.

## Test ve yayın öncesi kontrol

- Angular testleri: `npm test`
- Angular üretim derlemesi: `npm run build`
- API derlemesi: `dotnet build .\MarmaraUlasim.API`
- API ile PostgreSQL bağlantısı ve migration durumu ayrıca doğrulanmalıdır.
- İl, ilçe, mahalle ve durak sayıları gerçek API yanıtlarıyla karşılaştırılmalıdır.
- Harita, güzergâh, favoriler ve boş/hata durumları uçtan uca test edilmelidir.

## Güvenlik ve yapılandırma

- API CORS ayarı tüm kaynaklara açık değildir; yalnızca `Cors:AllowedOrigins` listesi kabul edilir.
- Veritabanı bağlantısı `ConnectionStrings__Default` ortam değişkeniyle verilir.
- Üretim dağıtımında HTTPS, gerçek API adresi, veritabanı yedekleri ve hata günlükleri ayrıca yapılandırılmalıdır.

## Durum

İyileştirmeler `fix/marmara-stability-from-master` dalında hazırlanıyor. Varsayılan `main` dalı şu an yalnızca kısa bir README içerdiği için, uygulama kodunun bulunduğu `master` tabanından ayrı bir çalışma dalı oluşturuldu. Bu değişiklikler yerel ortamda derlenip test edilmeden ana dala birleştirilmemelidir.

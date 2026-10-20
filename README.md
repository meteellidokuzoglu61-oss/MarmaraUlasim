# Marmara Ulaşım

Marmara Bölgesi'ndeki illerin coğrafi bilgilerini ve mevcut toplu taşıma verilerini tek uygulamada sunmayı hedefleyen Ionic/Angular + ASP.NET Core + PostgreSQL projesi.

## Mevcut özellikler

- Marmara Bölgesi'nin 11 ilini listeleme; ilçe ve mahalle verilerini Türkiye API üzerinden alma.
- Aktif otobüs hatlarını ve durakları listeleme, arama ve filtreleme.
- Hat ve durak favorilerini tarayıcıda saklama.
- Leaflet/OpenStreetMap üzerinde mevcut sefer güzergâhlarını gösterme.
- Kocaeli GTFS verisini uygulama başlangıcında içe aktarma.
- ASP.NET Core API ve /health sağlık kontrolü.

> **Veri kapsamı:** Bu sürümde GTFS ile toplu taşıma güzergâhı içe aktarımı Kocaeli ile sınırlıdır. Marmara'nın 11 ilinin tamamı için canlı araç konumu, bütün şehirlerin hatları ve uçtan uca yolculuk planlama özelliği henüz tamamlanmış değildir. Yayın açıklamalarında bu özellikler varmış gibi belirtilmemelidir.

## Teknoloji yığını

- Frontend: Ionic, Angular, TypeScript, Leaflet
- Backend: ASP.NET Core 10, Entity Framework Core
- Veri tabanı: PostgreSQL
- Ulaşım verisi: GTFS (mevcut içe aktarıcı Kocaeli kaynağını kullanır)

## Gereksinimler

- Node.js sürümü, kullanılan Angular CLI ve package-lock.json ile uyumlu olmalıdır.
- .NET 10 SDK
- PostgreSQL
- Git

## Yerel geliştirme

### 1. Veritabanı bağlantısını yerel gizli ayar olarak tanımla

Proje kökünde PowerShell aç:

    dotnet user-secrets init --project .\MarmaraUlasim.API
    dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Port=5432;Database=MarmaraUlasim;Username=postgres;Password=POSTGRES_PAROLAN" --project .\MarmaraUlasim.API

POSTGRES_PAROLAN yerine kendi PostgreSQL parolanı yaz. Gerçek parolayı Git'e veya bu README'ye ekleme.

### 2. API'yi çalıştır

    dotnet run --project .\MarmaraUlasim.API

API başlangıçta EF Core migration'larını uygular. Geliştirme ortamında Swagger arayüzü /swagger yolunda açılır. Geliştirme portu farklıysa frontend'in src/environments/environment.ts dosyasındaki API adresini güncelle.

### 3. Frontend'i çalıştır

Proje kökünde ikinci bir terminal aç:

    npm ci
    npm start

Angular geliştirme sunucusu varsayılan olarak http://localhost:4200 adresinde açılır. Geliştirme API adresi http://localhost:5197/api olarak yapılandırılmıştır.

## Üretim yayını

Üretim derlemesi, aynı alan adı üzerinden /api yolunun ASP.NET Core API'ye ulaştığı şekilde tasarlanmıştır. Backend wwwroot içindeki frontend dosyalarını sunar; bu nedenle tek alan adı / aynı kaynaklı kurulum en kolay seçenektir.

### Frontend'i backend'e paketle

Proje kökünde PowerShell:

    npm ci
    npm run build
    if (Test-Path .\MarmaraUlasim.API\wwwroot) {
      Remove-Item .\MarmaraUlasim.API\wwwroot -Recurse -Force
    }
    New-Item -ItemType Directory -Path .\MarmaraUlasim.API\wwwroot -Force | Out-Null
    Copy-Item .\www\* .\MarmaraUlasim.API\wwwroot\ -Recurse -Force
    dotnet publish .\MarmaraUlasim.API\MarmaraUlasim.API.csproj -c Release -o .\publish

Yayın ortamında en az şu ortam değişkenlerini tanımla:

- ASPNETCORE_ENVIRONMENT=Production
- ConnectionStrings__Default: yayın veritabanının PostgreSQL bağlantı dizesi
- Admin__ApiKey: /api/Veri/* veri aktarım uç noktaları için güçlü ve rastgele bir anahtar
- Cors__AllowedOrigins__0: frontend farklı bir alan adında barındırılıyorsa tam frontend origin'i (ör. https://site-ornegi.example)

Frontend ve API farklı alan adlarında yayımlanacaksa derlemeden önce src/environments/environment.prod.ts içindeki apiUrl değerini gerçek API adresine ayarla ve API tarafında izin verilen CORS origin'ini tanımla. Örnek alan adını olduğu gibi kullanma.

### Kontrol

- /health adresi uygulamanın sağlık kontrolünü döndürür.
- /swagger yalnızca geliştirme ortamında açıktır.
- Üretimde veri aktarım istekleri X-Admin-Key başlığı ile Admin__ApiKey ayarını göndermelidir.
- İlk çalıştırmada PostgreSQL sunucusuna erişilebildiğinden emin ol. Kocaeli GTFS içe aktarıcısı başlangıçta harici kaynaktan veri indirdiği için dış ağ erişimi gerekir.

## API uç noktaları

- GET /api/Iller
- GET /api/Iller/{id}
- GET /api/Ilceler/il/{ilId}
- GET /api/Mahalleler/ilce/{ilceId}
- GET /api/Hatlar
- GET /api/Duraklar
- GET /api/Duraklar/seferler
- GET /api/Duraklar/sefer/{seferKodu}/guzergah
- GET /health

Veri aktarım uç noktaları POST /api/Veri/* yalnızca geliştirme ortamında veya üretimde doğru X-Admin-Key sağlandığında çağrılabilir.

## Yayın notu

GitHub deposunda uygulamanın kaynak kodu master dalındadır; main dalı yalnızca başlangıç README dosyasını içeriyordu. Yayın veya otomatik dağıtım yapılandırırken doğru kaynak dalını seç. Bu dal ayrımı yayın öncesinde birleştirilmeli veya deponun varsayılan dalı kaynak kodu içeren dala alınmalıdır.

## Docker ile yayına hazırlama

Depo kökünde:

    docker build -t marmara-ulasim .
    docker run --rm -p 8080:8080 \
      -e ASPNETCORE_ENVIRONMENT=Production \
      -e ConnectionStrings__Default="Host=HOST;Port=5432;Database=DB_ADI;Username=DB_KULLANICISI;Password=YENI_GIZLI_PAROLA;SSL Mode=Require" \
      -e Admin__ApiKey="UZUN_RASTGELE_YONETICI_ANAHTARI" \
      marmara-ulasim

Yer tutucuları gerçek üretim veritabanı bilgileriyle değiştir. Bu komutta gerçek sırları kaynak dosyaya veya Git'e kaydetme. Barındırma sağlayıcısında aynı değerleri platformun Environment Variables / Secrets alanına ekle.

Uygulama 8080 portunu dinler. Yayın platformunda dış HTTPS bağlantısını platformun TLS/proxy katmanı yönetmelidir. Üretim veritabanı internetten erişilebilir olacaksa yalnızca sağlayıcının izin verdiği ağlardan erişim aç; güçlü parola ve TLS kullan.

> Docker imajı derlenebilir olsa bile gerçek yayından önce sağlayıcı ortamında PostgreSQL bağlantısını, /health kontrolünü, ilçe/mahalle veri aktarımını ve Kocaeli GTFS içe aktarımını doğrula.

# Setur Contacts - Docker Kurulumu

Bu proje Docker kullanarak tüm servisleri (PostgreSQL, Redis, Kafka, Zookeeper) ve uygulamaları (Contact API, Report API, Blazor App) birlikte çalıştırmak için yapılandırılmıştır.

## Gereksinimler

- Docker Desktop
- Docker Compose

## Hızlı Başlangıç

### 1. Tüm Servisleri Başlatma

```bash
docker-compose up -d
```

Bu komut aşağıdaki servisleri başlatır:
- **PostgreSQL** (Port: 5432)
- **Redis** (Port: 6379)
- **Zookeeper** (Port: 2181)
- **Kafka** (Port: 9092, 29092)
- **Contact API** (Port: 5001)
- **Report API** (Port: 5002)
- **Blazor App** (Port: 5000)
- **Kafdrop** (Port: 9000) - Kafka UI

### 2. Uygulamalara Erişim

- **Blazor Uygulaması**: http://localhost:5000
- **Contact API**: http://localhost:5001
- **Report API**: http://localhost:5002
- **Kafka UI (Kafdrop)**: http://localhost:9000

### 3. Servisleri Durdurma

```bash
docker-compose down
```

### 4. Tüm Verileri Silme (Dikkatli Kullanın!)

```bash
docker-compose down -v
```

## Servis Detayları

### Veritabanı
- **PostgreSQL 15**
- **Database**: setur_contacts (Contact API için)
- **Database**: setur_report (Report API için)
- **Username**: setur
- **Password**: Setur123!
- **Port**: 5432

### Cache
- **Redis 7**
- **Port**: 6379

### Message Broker
- **Apache Kafka 7.4.0**
- **Zookeeper**: 2181
- **Kafka External**: 9092
- **Kafka Internal**: 29092

### Uygulamalar
- **Contact API**: .NET 9.0 Web API (Port: 8080 container içinde)
- **Report API**: .NET 9.0 Web API (Port: 8080 container içinde, SignalR Hub ile)
- **Blazor App**: .NET 9.0 Blazor Server (Port: 8080 container içinde)

## Docker Compose Servisleri

### Infrastructure Services
- `postgres`: PostgreSQL veritabanı
- `redis`: Redis cache
- `zookeeper`: Kafka için Zookeeper
- `kafka`: Apache Kafka message broker
- `kafdrop`: Kafka UI (opsiyonel)

### Application Services
- `contact-api`: Contact API (Port 5001 → 8080)
- `report-api`: Report API (Port 5002 → 8080)
- `blazor-app`: Blazor Server App (Port 5000 → 8080)

## Network Yapılandırması

Tüm servisler `setur-network` adlı bir Docker network'ünde çalışır. Bu sayede servisler birbirleriyle container isimleri üzerinden iletişim kurabilir:

- Contact API → PostgreSQL: `postgres:5432`
- Report API → PostgreSQL: `postgres:5432`
- Report API → Kafka: `kafka:29092`
- Blazor App → Contact API: `contact-api:8080`
- Blazor App → Report API: `report-api:8080`
- Report API → Contact API: `contact-api:8080`

## Environment Variables

### Contact API
```yaml
- ASPNETCORE_ENVIRONMENT=Development
- ConnectionStrings__DefaultConnection=Host=postgres;Database=setur_contacts;Username=setur;Password=Setur123!
- Kafka__BootstrapServers=kafka:29092
- Redis__ConnectionString=redis:6379
```

### Report API
```yaml
- ASPNETCORE_ENVIRONMENT=Development
- ConnectionStrings__DefaultConnection=Host=postgres;Database=setur_report;Username=setur;Password=Setur123!
- Kafka__BootstrapServers=kafka:29092
- Redis__ConnectionString=redis:6379
- ContactApi__BaseUrl=http://contact-api:8080
```

### Blazor App
```yaml
- ASPNETCORE_ENVIRONMENT=Development
- ContactApi__BaseUrl=http://contact-api:8080
- ReportApi__BaseUrl=http://report-api:8080
- SignalR__ReportHubUrl=http://report-api:8080/reportHub
```

## Port Yapılandırması

### Container İçi Portlar
Tüm uygulamalar container içinde 8080 portunda çalışır:
- Contact API: `http://0.0.0.0:8080`
- Report API: `http://0.0.0.0:8080`
- Blazor App: `http://0.0.0.0:8080`

### Host Port Mapping
- Contact API: `localhost:5001` → `container:8080`
- Report API: `localhost:5002` → `container:8080`
- Blazor App: `localhost:5000` → `container:8080`

## Troubleshooting

### 1. Port Çakışması
Eğer portlar kullanımdaysa, `docker-compose.yml` dosyasındaki port mapping'leri değiştirin.

### 2. Veritabanı Bağlantı Hatası
```bash
# PostgreSQL container'ının durumunu kontrol edin
docker-compose logs postgres

# Veritabanına bağlanın
docker-compose exec postgres psql -U setur -d setur_contacts
# veya
docker-compose exec postgres psql -U setur -d setur_report
```

### 3. Kafka Bağlantı Hatası
```bash
# Kafka container'ının durumunu kontrol edin
docker-compose logs kafka

# Kafka UI'ı kontrol edin
# http://localhost:9000
```

### 4. Uygulama Logları
```bash
# Tüm logları görüntüle
docker-compose logs

# Belirli bir servisin loglarını görüntüle
docker-compose logs contact-api
docker-compose logs report-api
docker-compose logs blazor-app
```

### 5. Servisleri Yeniden Başlatma
```bash
# Belirli bir servisi yeniden başlat
docker-compose restart contact-api

# Tüm servisleri yeniden başlat
docker-compose restart
```

### 6. Container İçi Port Kontrolü
```bash
# Container'ın hangi portta çalıştığını kontrol et
docker exec setur-contacts-contact-api netstat -tlnp
docker exec setur-contacts-report-api netstat -tlnp
docker exec setur-contacts-blazor-app netstat -tlnp
```

## Development

### Yeni Özellik Ekleme
1. Kodunuzu geliştirin
2. Docker image'larını yeniden build edin:
   ```bash
   docker-compose build
   ```
3. Servisleri yeniden başlatın:
   ```bash
   docker-compose up -d
   ```

### Hot Reload (Geliştirme için)
Docker Compose ile hot reload için volume mapping ekleyebilirsiniz. `docker-compose.yml` dosyasındaki her uygulama servisine:

```yaml
volumes:
  - ./src:/src
```

## Production

Production ortamı için:
1. `ASPNETCORE_ENVIRONMENT=Production` olarak ayarlayın
2. Güvenli connection string'ler kullanın
3. HTTPS sertifikaları ekleyin
4. Logging seviyelerini ayarlayın
5. Resource limitleri ekleyin

## Monitoring

- **Kafka UI**: http://localhost:9000
- **Application Logs**: `docker-compose logs -f`
- **Resource Usage**: `docker stats`

## API Dokümantasyonu

- **Contact API Swagger**: http://localhost:5001
- **Report API Swagger**: http://localhost:5002

## Test Verisi Oluşturma

Contact API'de test verisi oluşturmak için:

### cURL ile
```bash
# Test verisi oluştur (100 kişi ve iletişim bilgileri)
curl -X POST http://localhost:5001/api/TestData/generate

# Test verisi temizle
curl -X DELETE http://localhost:5001/api/TestData/clear
```

### Swagger UI ile
1. Contact API Swagger UI'ını açın: http://localhost:5001
2. `/api/TestData/generate` endpoint'ini bulun
3. "Try it out" butonuna tıklayın
4. Execute butonuna tıklayın

### Test Verisi İçeriği
- **100 adet kişi** (rastgele isim ve soyisim)
- **Her kişi için 1-3 adet iletişim bilgisi** (telefon, e-posta, adres)
- **Şirket isimleri**: 5-10 karakter arası rastgele isimler
- **Lokasyon**: Sadece şehir isimleri

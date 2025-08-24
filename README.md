# Setur Contacts

Kişi ve iletişim bilgilerini yönetmek için geliştirilmiş modern bir web uygulaması.

## 🚀 Hızlı Başlangıç

### Docker ile Çalıştırma (Önerilen)
```bash
# Tüm servisleri başlat
docker-compose up -d

# Blazor uygulamasına eriş
# http://localhost:5000
```

Detaylı Docker kurulum rehberi için: [📚 Dokümantasyon](./docs/DOCKER_README.md)

### Manuel Çalıştırma
1. PostgreSQL, Redis ve Kafka servislerini başlatın
2. Contact API'yi çalıştırın (Port: 7001)
3. Report API'yi çalıştırın (Port: 5079)
4. Blazor uygulamasını çalıştırın (Port: 5000)

## 📋 Özellikler

- ✅ **Kişi Yönetimi**: Kişi ekleme, düzenleme, silme
- ✅ **İletişim Bilgileri**: Telefon, e-posta, adres yönetimi
- ✅ **Rapor Oluşturma**: Kişi listesi raporları
- ✅ **Real-time Bildirimler**: SignalR ile anlık güncellemeler
- ✅ **Toast Bildirimleri**: Kullanıcı dostu bildirimler
- ✅ **Pagination**: Sayfalı veri görüntüleme
- ✅ **Validation**: FluentValidation ile veri doğrulama
- ✅ **Error Handling**: Merkezi hata yönetimi
- ✅ **Docker Support**: Tam container desteği

## 🏗️ Mimari

- **Clean Architecture** prensipleri
- **Repository Pattern** ile veri erişimi
- **Service Layer** ile iş mantığı
- **Dependency Injection** ile gevşek bağlılık

## 🔧 Teknolojiler

- **Backend**: .NET 9.0, ASP.NET Core Web API
- **Frontend**: Blazor Server
- **Veritabanı**: PostgreSQL
- **Cache**: Redis
- **Message Broker**: Apache Kafka
- **Containerization**: Docker & Docker Compose
- **Testing**: xUnit, FluentAssertions, Moq
- **Validation**: FluentValidation
- **Mapping**: Mapster
- **Real-time**: SignalR

## 📁 Proje Yapısı

```
Setur.Contacts/
├── src/
│   ├── Setur.Contacts.Base/          # Ortak servisler ve middleware
│   ├── Setur.Contacts.Domain/        # Entity'ler ve DTO'lar
│   ├── Setur.Contacts.ContactApi/    # Contact API
│   ├── Setur.Contacts.ReportApi/     # Report API
│   └── Setur.Contacts.BlazorApp/     # Blazor UI
├── tests/
│   └── Setur.Contacts.Tests/         # Unit ve Integration testler
├── docs/                             # Dokümantasyon
└── docker-compose.yml               # Docker Compose yapılandırması
```

## 📚 Dokümantasyon

- [🐳 Docker Kurulum Rehberi](./docs/DOCKER_README.md)
- [📋 Proje Dokümantasyonu](./docs/README.md)

## 🧪 Test

```bash
# Testleri çalıştır
dotnet test

# Belirli test projesini çalıştır
dotnet test tests/Setur.Contacts.Tests/
```

## 📞 API Endpoints

### Contact API (Port: 5001)
- `GET /api/Contact` - Tüm kişileri listele
- `GET /api/Contact/paged` - Sayfalı kişi listesi
- `POST /api/Contact` - Yeni kişi ekle
- `PUT /api/Contact/{id}` - Kişi güncelle
- `DELETE /api/Contact/{id}` - Kişi sil

### Report API (Port: 5002)
- `GET /api/Report` - Tüm raporları listele
- `GET /api/Report/{id}` - Akıllı rapor getirme (Cache → DB → Metadata)
- `POST /api/Report` - Yeni rapor oluştur
- `DELETE /api/Report/{id}` - Rapor sil
- `POST /api/Report/{reportId}/save-permanently` - Raporu kalıcı kaydet

## 🧪 Test Verisi Oluşturma

### Docker ile Çalıştırırken
```bash
# Test verisi oluştur (100 kişi ve iletişim bilgileri)
curl -X POST http://localhost:5001/api/TestData/generate

# Test verisi temizle
curl -X DELETE http://localhost:5001/api/TestData/clear
```

### Manuel Çalıştırırken
```bash
# Test verisi oluştur (100 kişi ve iletişim bilgileri)
curl -X POST http://localhost:7001/api/TestData/generate

# Test verisi temizle
curl -X DELETE http://localhost:7001/api/TestData/clear
```

### Swagger UI ile
1. Contact API Swagger UI'ını açın: http://localhost:5001 (Docker) veya http://localhost:7001 (Manuel)
2. `/api/TestData/generate` endpoint'ini bulun
3. "Try it out" butonuna tıklayın
4. Execute butonuna tıklayın

## 📄 Lisans

Bu proje MIT lisansı altında lisanslanmıştır.
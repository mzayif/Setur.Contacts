# Setur Contacts - Dokümantasyon

Bu klasör Setur Contacts projesinin dokümantasyonunu içerir.

## 🏗️ Mimari Genel Bakış

Bu proje **Microservice Mimarisi** ile geliştirilmiştir ve aşağıdaki özelliklere sahiptir:

### 🔄 Microservice Yapısı
- **Contact API**: Kişi ve iletişim bilgileri yönetimi
- **Report API**: Rapor oluşturma ve yönetimi
- **Blazor UI**: Kullanıcı arayüzü

### 🗄️ Veritabanı Ayrımı
- **Contact API**: `setur_contacts` veritabanı (kişi ve iletişim bilgileri)
- **Report API**: `setur_report` veritabanı (rapor verileri)
- Her servis kendi veritabanına sahip, bağımsız çalışır

### 📡 Asenkron İletişim
- **Apache Kafka**: Servisler arası mesajlaşma
- **Redis**: Cache ve geçici veri saklama
- **SignalR**: Real-time UI güncellemeleri

### ⚡ Real-time Bildirimler
- Rapor durumu değişiklikleri anında UI'a iletilir
- Kullanıcı sayfa yenilemeden güncellemeleri görür
- SignalR Hub ile WebSocket bağlantısı

### 🗄️ Rapor Cache Yönetimi
- **Redis Cache**: Rapor sonuçları 24 saat boyunca Redis'te saklanır
- **Akıllı Getirme**: Rapor görüntülenirken Cache → Database → Metadata sırasıyla kontrol edilir
- **Kalıcı Kaydetme**: Kullanıcı isterse raporu kalıcı olarak veritabanına kaydedebilir
- **TTL Yönetimi**: 24 saat sonra cache otomatik temizlenir

## 📚 Dokümantasyon Listesi

### 🐳 Docker Kurulumu
- **[DOCKER_README.md](./DOCKER_README.md)** - Docker Compose ile tüm servisleri çalıştırma rehberi

## 🚀 Hızlı Başlangıç

### Docker ile Çalıştırma
1. [Docker Kurulum Rehberi](./DOCKER_README.md) dosyasını inceleyin
2. `docker-compose up -d` komutu ile tüm servisleri başlatın
3. http://localhost:5000 adresinden Blazor uygulamasına erişin

### Manuel Çalıştırma
1. PostgreSQL, Redis ve Kafka servislerini başlatın
2. Contact API'yi çalıştırın (Port: 7001)
3. Report API'yi çalıştırın (Port: 5079)
4. Blazor uygulamasını çalıştırın (Port: 5000)

## 🧪 Test Verisi Oluşturma

### Docker ile Çalıştırırken
```bash
# Test verisi oluştur (100 kişi ve iletişim bilgileri)
curl -X POST http://localhost:5001/api/TestData/generate

# Test verisi temizle
curl -X DELETE http://localhost:5001/api/TestData/clear


### Manuel Çalıştırırken
```bash
# Test verisi oluştur (100 kişi ve iletişim bilgileri)
curl -X POST http://localhost:7001/api/TestData/generate

# Test verisi temizle
curl -X DELETE http://localhost:7001/api/TestData/clear


### Swagger UI ile
1. Contact API Swagger UI'ını açın: http://localhost:5001 (Docker) veya http://localhost:7001 (Manuel)
2. `/api/TestData/generate` endpoint'ini bulun
3. "Try it out" butonuna tıklayın
4. Execute butonuna tıklayın

## 📋 Proje Yapısı

```
Setur.Contacts/
├── src/
│   ├── Setur.Contacts.Base/          # Ortak servisler ve middleware
│   ├── Setur.Contacts.Domain/        # Entity'ler ve DTO'lar
│   ├── Setur.Contacts.ContactApi/    # Contact API (setur_contacts DB)
│   ├── Setur.Contacts.ReportApi/     # Report API (setur_report DB)
│   ├── Setur.Contacts.MessageBus/    # Kafka mesajlaşma servisleri
│   └── Setur.Contacts.BlazorApp/     # Blazor UI (SignalR client)
├── tests/
│   └── Setur.Contacts.Tests/         # Unit ve Integration testler
├── docs/                             # Dokümantasyon
└── docker-compose.yml               # Docker Compose yapılandırması
```

### 🔄 Servis İletişim Akışı
1. **Blazor UI** → **Contact API** (HTTP - kişi işlemleri)
2. **Blazor UI** → **Report API** (HTTP - rapor oluşturma)
3. **Report API** → **Contact API** (HTTP - kişi verilerini çekme)
4. **Report API** → **Kafka** (mesaj gönderme)
5. **Kafka** → **Report API** (Background Service - mesaj işleme)
6. **Report API** → **Redis** (rapor sonuçlarını cache'e kaydetme)
7. **Report API** → **Blazor UI** (SignalR - real-time bildirimler)

### 📊 Rapor İşleme Akışı
1. **Rapor Oluşturma**: Kullanıcı rapor oluşturur
2. **Asenkron İşleme**: Kafka üzerinden background service'e gönderilir
3. **Veri İşleme**: Contact API'den kişi verileri çekilir
4. **Cache Kaydetme**: Sonuçlar Redis'e 24 saat TTL ile kaydedilir
5. **Bildirim**: SignalR ile kullanıcıya bildirim gönderilir
6. **Görüntüleme**: Cache → Database → Metadata sırasıyla kontrol edilir
7. **Kalıcı Kaydetme**: Kullanıcı isterse veritabanına kalıcı kaydeder

## 🔧 Teknolojiler

### 🎯 Backend & Frontend
- **Backend**: .NET 9.0, ASP.NET Core Web API
- **Frontend**: Blazor Server
- **Validation**: FluentValidation
- **Mapping**: Mapster

### 🗄️ Veritabanı & Cache
- **Veritabanı**: PostgreSQL (ayrı veritabanları)
- **Cache**: Redis (rapor cache sistemi - 24 saat TTL)

### 📡 İletişim & Mesajlaşma
- **Message Broker**: Apache Kafka (asenkron iletişim)
- **Real-time**: SignalR (WebSocket bağlantıları)

### 🐳 Deployment & Testing
- **Containerization**: Docker & Docker Compose
- **Testing**: xUnit, FluentAssertions, Moq

## 📞 İletişim

Proje ile ilgili sorularınız için dokümantasyonu inceleyin veya geliştirici ekibi ile iletişime geçin.

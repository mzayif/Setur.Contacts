using Setur.Contacts.MessageBus.Models;

namespace Setur.Contacts.MessageBus.Services;

/// <summary>
/// Kafka Consumer Service Interface
/// 
/// Bu servis, Kafka'dan gelen rapor isteklerini dinler ve işler.
/// 
/// İş Akışı:
/// 1. Kafka topic'ini dinlemeye başlar
/// 2. Gelen mesajları alır
/// 3. ReportProcessorService'e gönderir
/// 4. İşlem sonucunu Kafka'ya geri gönderir
/// 
/// Kullanım Amacı:
/// - Asenkron mesaj işleme
/// - Yüksek performans
/// - Hata durumunda mesaj tekrar işleme
/// - Ölçeklenebilirlik
/// </summary>
public interface IKafkaConsumerService
{
    /// <summary>
    /// Kafka topic'ini dinlemeye başlar
    /// </summary>
    /// <param name="cancellationToken">İptal token'ı</param>
    /// <returns>Dinleme işlemi</returns>
    Task StartConsumingAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Kafka dinleme işlemini durdurur
    /// </summary>
    void StopConsuming();
}

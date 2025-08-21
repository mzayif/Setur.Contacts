using Setur.Contacts.MessageBus.Models;

namespace Setur.Contacts.MessageBus.Services;

/// <summary>
/// Kafka Producer Service Interface
/// 
/// Bu servis, ReportApi'den gelen rapor isteklerini Kafka message queue'ya gönderir.
/// 
/// İş Akışı:
/// 1. ReportApi'den rapor oluşturma isteği gelir
/// 2. Bu servis isteği Kafka'ya gönderir
/// 3. Kafka Consumer (ReportApi'de) mesajı alır ve işler
/// 
/// Kullanım Amacı:
/// - Asenkron rapor işleme
/// - Mikroservisler arası iletişim
/// - Yük dengeleme
/// - Hata toleransı
/// </summary>
public interface IKafkaProducerService
{
    /// <summary>
    /// Rapor isteğini Kafka topic'ine gönderir
    /// </summary>
    /// <param name="message">Gönderilecek rapor isteği mesajı</param>
    /// <returns>Gönderim başarılı ise true, değilse false</returns>
    Task<bool> SendReportRequestAsync(ReportRequestMessage message);
}

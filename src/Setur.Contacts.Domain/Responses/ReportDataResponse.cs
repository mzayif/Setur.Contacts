using Setur.Contacts.Domain.Enums;

namespace Setur.Contacts.Domain.Responses
{
    /// <summary>
    /// Rapor verisi - Tüm rapor türleri için ortak model
    /// </summary>
    public class ReportDataResponse
    {
        /// <summary>
        /// Rapor türü
        /// </summary>
        public ReportType ReportType { get; set; }

        /// <summary>
        /// Filtre değerleri (lokasyonlar, şirketler vs.)
        /// </summary>
        public List<string> Filters { get; set; } = new();

        /// <summary>
        /// Toplam kişi sayısı
        /// </summary>
        public int TotalPersonCount { get; set; }

        /// <summary>
        /// Toplam telefon sayısı
        /// </summary>
        public int TotalPhoneCount { get; set; }

        /// <summary>
        /// Toplam email sayısı
        /// </summary>
        public int TotalEmailCount { get; set; }

        /// <summary>
        /// Toplam lokasyon sayısı
        /// </summary>
        public int TotalLocationCount { get; set; }

        /// <summary>
        /// En çok kişi bulunan şirketler
        /// </summary>
        public List<string> TopCompanies { get; set; } = new();

        /// <summary>
        /// En çok kişi bulunan lokasyonlar
        /// </summary>
        public List<string> TopLocations { get; set; } = new();

        /// <summary>
        /// Detay verileri - Lokasyon bazlı alt toplamlar
        /// </summary>
        public List<ReportDetailData> Details { get; set; } = new();
    }

    /// <summary>
    /// Rapor detay verisi - Lokasyon bazlı alt toplamlar
    /// </summary>
    public class ReportDetailData
    {
        /// <summary>
        /// Lokasyon adı
        /// </summary>
        public string Location { get; set; } = string.Empty;

        /// <summary>
        /// Bu lokasyondaki kişi sayısı
        /// </summary>
        public int PersonCount { get; set; }

        /// <summary>
        /// Bu lokasyondaki telefon sayısı
        /// </summary>
        public int PhoneCount { get; set; }

        /// <summary>
        /// Bu lokasyondaki email sayısı
        /// </summary>
        public int EmailCount { get; set; }
    }
}

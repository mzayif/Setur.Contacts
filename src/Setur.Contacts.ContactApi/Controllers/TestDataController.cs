using Microsoft.AspNetCore.Mvc;
using Setur.Contacts.Base.Interfaces;
using Setur.Contacts.Base.Results;
using Setur.Contacts.ContactApi.Services;
using Setur.Contacts.Domain.Enums;
using Setur.Contacts.Domain.Requests;

namespace Setur.Contacts.ContactApi.Controllers;

/// <summary>
/// Test verisi oluşturmak için kullanılan controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TestDataController : ControllerBase
{
    private readonly IContactService _contactService;
    private readonly ICommunicationInfoService _communicationInfoService;
    private readonly ILoggerService _loggerService;

    public TestDataController(
        IContactService contactService,
        ICommunicationInfoService communicationInfoService,
        ILoggerService loggerService)
    {
        _contactService = contactService;
        _communicationInfoService = communicationInfoService;
        _loggerService = loggerService;
    }

    /// <summary>
    /// Belirtilen sayıda test verisi oluşturur
    /// </summary>
    /// <param name="count">Oluşturulacak contact sayısı (varsayılan: 10)</param>
    /// <returns>Oluşturulan veri sayısı</returns>
    [HttpPost("generate")]
    public async Task<ActionResult<SuccessDataResult<int>>> GenerateTestData([FromQuery] int count = 10)
    {
        try
        {
            _loggerService.LogInformation($"Test verisi oluşturma başlatıldı. Sayı: {count}");

            var createdCount = 0;
            var random = new Random();

            // Türkçe isimler listesi
            var firstNames = new[] { "Ahmet", "Mehmet", "Ali", "Ayşe", "Fatma", "Zeynep", "Mustafa", "Hasan", "Hüseyin", "İbrahim", "Emine", "Hatice", "Elif", "Meryem", "Sevgi", "Gül", "Can", "Deniz", "Ege", "Yağmur" };
            var lastNames = new[] { "Yılmaz", "Kaya", "Demir", "Çelik", "Şahin", "Yıldız", "Yıldırım", "Özkan", "Aydın", "Özdemir", "Arslan", "Doğan", "Kılıç", "Aslan", "Çetin", "Erdoğan", "Koç", "Kurt", "Özkan", "Şen" };

            // Şirket isimleri listesi (5-10 karakter arası)
            var companies = new[] { "Setur", "TechCorp", "DataSoft", "NetLink", "CloudSys", "WebPro", "DigiTech", "SmartSol", "InnoLab", "FutureX", "CodeHub", "DevTeam", "AppWorks", "SoftCore", "ByteLab", "PixelPro", "LogicSys", "CyberNet", "InfoTech", "DataCore" };

            for (int i = 0; i < count; i++)
            {
                // Rastgele contact oluştur
                var firstName = firstNames[random.Next(firstNames.Length)];
                var lastName = lastNames[random.Next(lastNames.Length)];
                var company = companies[random.Next(companies.Length)];

                var createContactRequest = new CreateContactRequest
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Company = company
                };

                var contactResult = await _contactService.CreateContactAsync(createContactRequest);
                if (contactResult.Success)
                {
                    createdCount++;

                    // Her contact için 1-3 arası communication info oluştur
                    var communicationCount = random.Next(1, 4);
                    for (int j = 0; j < communicationCount; j++)
                    {
                        try
                        {
                            var communicationType = (CommunicationType)random.Next(1, 4); // Email, Phone, Location
                            string value;

                            switch (communicationType)
                            {
                                case CommunicationType.Email:
                                    value = $"{firstName.ToLower()}.{lastName.ToLower()}@example.com";
                                    break;
                                case CommunicationType.Phone:
                                    value = $"05{random.Next(10, 99)} {random.Next(100, 999)} {random.Next(10, 99)} {random.Next(10, 99)}";
                                    break;
                                case CommunicationType.Location:
                                    var cities = new[] { "İstanbul", "Ankara", "İzmir", "Bursa", "Antalya", "Adana", "Konya", "Gaziantep", "Mersin", "Diyarbakır" };
                                    var city = cities[random.Next(cities.Length)];
                                    value = city;
                                    break;
                                default:
                                    value = "Test verisi";
                                    break;
                            }

                            var createCommunicationRequest = new CreateCommunicationInfoRequest
                            {
                                ContactId = Guid.Parse(contactResult.ResponseId),
                                Type = communicationType,
                                Value = value
                            };

                            await _communicationInfoService.CreateCommunicationInfoAsync(createCommunicationRequest);

                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                }
            }

            _loggerService.LogInformation($"Test verisi oluşturma tamamlandı. Oluşturulan contact sayısı: {createdCount}");

            return Ok(new SuccessDataResult<int>(createdCount, $"{createdCount} adet test verisi başarıyla oluşturuldu."));
        }
        catch (Exception ex)
        {
            _loggerService.LogError("Test verisi oluşturulurken hata oluştu", ex);
            return StatusCode(500, new ErrorResponse("Test verisi oluşturulurken bir hata oluştu"));
        }
    }

    /// <summary>
    /// Tüm verileri temizler
    /// </summary>
    /// <returns>Silinen veri sayısı</returns>
    [HttpDelete("clear")]
    public async Task<ActionResult<SuccessDataResult<int>>> ClearTestData()
    {
        try
        {
            _loggerService.LogInformation("Test verisi temizleme başlatıldı");

            // Tüm contactları al
            var contactsResult = await _contactService.GetAllContactsAsync();
            if (!contactsResult.Success || contactsResult.Data == null)
            {
                return Ok(new SuccessDataResult<int>(0, "Silinecek veri bulunamadı."));
            }

            var deletedCount = 0;
            foreach (var contact in contactsResult.Data)
            {
                var deleteResult = await _contactService.DeleteContactAsync(contact.Id);
                if (deleteResult.Success)
                {
                    deletedCount++;
                }
            }

            _loggerService.LogInformation($"Test verisi temizleme tamamlandı. Silinen contact sayısı: {deletedCount}");

            return Ok(new SuccessDataResult<int>(deletedCount, $"{deletedCount} adet test verisi başarıyla silindi."));
        }
        catch (Exception ex)
        {
            _loggerService.LogError("Test verisi temizlenirken hata oluştu", ex);
            return StatusCode(500, new ErrorResponse("Test verisi temizlenirken bir hata oluştu"));
        }
    }
}



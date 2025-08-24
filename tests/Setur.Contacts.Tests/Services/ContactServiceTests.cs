using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Setur.Contacts.Base.Exceptions;
using Setur.Contacts.ContactApi.Repositories;
using Setur.Contacts.ContactApi.Services;
using Setur.Contacts.Domain.Entities;
using Setur.Contacts.Domain.Enums;
using Setur.Contacts.Domain.Requests;

namespace Setur.Contacts.Tests.Services;

public class ContactServiceTests : TestBase, IDisposable
{
    private readonly ContactService _contactService;
    private readonly ContactRepository _contactRepository;

    public ContactServiceTests()
    {
        _contactRepository = GetService<ContactRepository>();
        _contactService = new ContactService(_contactRepository);
    }

    /// <summary>
    /// Tüm kişileri getirme işleminin başarılı olmasını test eder
    /// </summary>
    [Fact]
    public async Task GetAllContactsAsync_ShouldReturnAllContacts()
    {
        // Arrange
        var contacts = new List<Contact>
        {
            new() { FirstName = "Ahmet", LastName = "Yılmaz", Company = "Setur" },
            new() { FirstName = "Mehmet", LastName = "Kaya", Company = "TechCorp" }
        };

        await DbContext.Contacts.AddRangeAsync(contacts);
        await SaveChangesAsync();

        // Act
        var result = await _contactService.GetAllContactsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().HaveCount(2);
        result.Data.Should().Contain(c => c.FirstName == "Ahmet" && c.LastName == "Yılmaz");
        result.Data.Should().Contain(c => c.FirstName == "Mehmet" && c.LastName == "Kaya");
    }

    /// <summary>
    /// Geçerli ID ile kişi getirme işleminin başarılı olmasını test eder
    /// </summary>
    [Fact]
    public async Task GetContactByIdAsync_WithValidId_ShouldReturnContact()
    {
        // Arrange
        var contact = new Contact { FirstName = "Ahmet", LastName = "Yılmaz", Company = "Setur" };
        await DbContext.Contacts.AddAsync(contact);
        await SaveChangesAsync();

        // Act
        var result = await _contactService.GetContactByIdAsync(contact.Id);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.FirstName.Should().Be("Ahmet");
        result.Data.LastName.Should().Be("Yılmaz");
        result.Data.Company.Should().Be("Setur");
    }

    /// <summary>
    /// Geçersiz ID ile kişi getirme işleminde NotFoundException fırlatılmasını test eder
    /// </summary>
    [Fact]
    public async Task GetContactByIdAsync_WithInvalidId_ShouldThrowNotFoundException()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act & Assert
        await _contactService.Invoking(s => s.GetContactByIdAsync(invalidId))
            .Should().ThrowAsync<NotFoundException>()
            .WithMessage("Kişi Bulunamadı");
    }

    /// <summary>
    /// Geçerli verilerle kişi oluşturma işleminin başarılı olmasını test eder
    /// </summary>
    [Fact]
    public async Task CreateContactAsync_WithValidData_ShouldCreateContact()
    {
        // Arrange
        var request = new CreateContactRequest
        {
            FirstName = "Ahmet",
            LastName = "Yılmaz",
            Company = "Setur"
        };

        // Act
        var result = await _contactService.CreateContactAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Contain("başarıyla oluşturuldu");

        // Verify contact was saved to database
        var savedContact = await DbContext.Contacts.FirstOrDefaultAsync(c => c.FirstName == "Ahmet");
        savedContact.Should().NotBeNull();
        savedContact!.FirstName.Should().Be("Ahmet");
        savedContact.LastName.Should().Be("Yılmaz");
        savedContact.Company.Should().Be("Setur");
    }

    /// <summary>
    /// Geçerli verilerle kişi güncelleme işleminin başarılı olmasını test eder
    /// </summary>
    [Fact]
    public async Task UpdateContactAsync_WithValidData_ShouldUpdateContact()
    {
        // Arrange
        var contact = new Contact { FirstName = "Ahmet", LastName = "Yılmaz", Company = "Setur" };
        await DbContext.Contacts.AddAsync(contact);
        await SaveChangesAsync();

        var request = new UpdateContactRequest
        {
            Id = contact.Id,
            FirstName = "Mehmet",
            LastName = "Kaya",
            Company = "TechCorp"
        };

        // Act
        var result = await _contactService.UpdateContactAsync(contact.Id, request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Contain("başarıyla güncellendi");

        // Verify contact was updated in database
        var updatedContact = await DbContext.Contacts.FindAsync(contact.Id);
        updatedContact.Should().NotBeNull();
        updatedContact!.FirstName.Should().Be("Mehmet");
        updatedContact.LastName.Should().Be("Kaya");
        updatedContact.Company.Should().Be("TechCorp");
    }

    /// <summary>
    /// Geçersiz ID ile kişi güncelleme işleminde NotFoundException fırlatılmasını test eder
    /// </summary>
    [Fact]
    public async Task UpdateContactAsync_WithInvalidId_ShouldThrowNotFoundException()
    {
        // Arrange
        var invalidId = Guid.NewGuid();
        var request = new UpdateContactRequest
        {
            Id = invalidId,
            FirstName = "Mehmet",
            LastName = "Kaya",
            Company = "TechCorp"
        };

        // Act & Assert
        await _contactService.Invoking(s => s.UpdateContactAsync(invalidId, request))
            .Should().ThrowAsync<NotFoundException>()
            .WithMessage("Kişi bulunamadı");
    }

    /// <summary>
    /// Geçerli ID ile kişi silme işleminin başarılı olmasını test eder
    /// </summary>
    [Fact]
    public async Task DeleteContactAsync_WithValidId_ShouldDeleteContact()
    {
        // Arrange
        var contact = new Contact { FirstName = "Ahmet", LastName = "Yılmaz", Company = "Setur" };
        await DbContext.Contacts.AddAsync(contact);
        await SaveChangesAsync();

        // Act
        var result = await _contactService.DeleteContactAsync(contact.Id);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Contain("başarıyla silindi");

        // Verify contact was deleted from database
        var deletedContact = await DbContext.Contacts.FindAsync(contact.Id);
        deletedContact.Should().BeNull();
    }

    /// <summary>
    /// Geçersiz ID ile kişi silme işleminde NotFoundException fırlatılmasını test eder
    /// </summary>
    [Fact]
    public async Task DeleteContactAsync_WithInvalidId_ShouldThrowNotFoundException()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act & Assert
        await _contactService.Invoking(s => s.DeleteContactAsync(invalidId))
            .Should().ThrowAsync<NotFoundException>()
            .WithMessage("Kişi bulunamadı");
    }

    /// <summary>
    /// Sayfalama ile kişi getirme işleminin doğru sonuçlar döndürmesini test eder
    /// </summary>
    [Fact]
    public async Task GetContactsPagedAsync_ShouldReturnPagedResults()
    {
        // Arrange
        var contacts = new List<Contact>();
        for (int i = 1; i <= 25; i++)
        {
            contacts.Add(new Contact
            {
                FirstName = $"Kişi{i}",
                LastName = $"Soyad{i}",
                Company = $"Şirket{i}"
            });
        }

        await DbContext.Contacts.AddRangeAsync(contacts);
        await SaveChangesAsync();

        var request = new PagedRequest
        {
            PageNumber = 2,
            PageSize = 10
        };

        // Act
        var result = await _contactService.GetContactsPagedAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().HaveCount(10);
        result.TotalCount.Should().Be(25);
        result.PageNumber.Should().Be(2);
        result.PageSize.Should().Be(10);
        result.TotalPages.Should().Be(3);
        result.HasPreviousPage.Should().BeTrue();
        result.HasNextPage.Should().BeTrue();
    }

    /// <summary>
    /// Boş veritabanında sayfalama ile kişi getirme işleminin boş sonuç döndürmesini test eder
    /// </summary>
    [Fact]
    public async Task GetContactsPagedAsync_WithEmptyDatabase_ShouldReturnEmptyResult()
    {
        // Arrange
        var request = new PagedRequest
        {
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await _contactService.GetContactsPagedAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalPages.Should().Be(0);
        result.HasPreviousPage.Should().BeFalse();
        result.HasNextPage.Should().BeFalse();
    }

    /// <summary>
    /// Lokasyon bazlı rapor verilerinin doğru hesaplanmasını test eder
    /// </summary>
    [Fact]
    public async Task GetReportDataAsync_WithLocationBasedReport_ShouldReturnCorrectData()
    {
        // Arrange
        var contact1 = new Contact { FirstName = "Ahmet", LastName = "Yılmaz", Company = "Setur" };
        var contact2 = new Contact { FirstName = "Mehmet", LastName = "Kaya", Company = "TechCorp" };
        
        await DbContext.Contacts.AddRangeAsync(contact1, contact2);
        await SaveChangesAsync();

        var communicationInfos = new List<CommunicationInfo>
        {
            new() { ContactId = contact1.Id, Type = CommunicationType.Location, Value = "İstanbul" },
            new() { ContactId = contact1.Id, Type = CommunicationType.Phone, Value = "5320001122" },
            new() { ContactId = contact1.Id, Type = CommunicationType.Email, Value = "ahmet@setur.com" },
            new() { ContactId = contact2.Id, Type = CommunicationType.Location, Value = "İstanbul" },
            new() { ContactId = contact2.Id, Type = CommunicationType.Phone, Value = "5329999999" }
        };

        await DbContext.CommunicationInfos.AddRangeAsync(communicationInfos);
        await SaveChangesAsync();

        var filters = new List<string> { "İstanbul" };

        // Act
        var result = await _contactService.GetReportDataAsync(ReportType.LocationBased, filters);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.TotalPersonCount.Should().Be(2);
        result.Data.TotalPhoneCount.Should().Be(2);
        result.Data.TotalEmailCount.Should().Be(1);
        result.Data.TotalLocationCount.Should().Be(1);
        result.Data.Details.Should().HaveCount(1);
        result.Data.Details!.First().Location.Should().Be("İstanbul");
        result.Data.Details.First().PersonCount.Should().Be(2);
        result.Data.Details.First().PhoneCount.Should().Be(2);
        result.Data.Details.First().EmailCount.Should().Be(1);
    }

    public new void Dispose()
    {
        base.Dispose();
    }
}

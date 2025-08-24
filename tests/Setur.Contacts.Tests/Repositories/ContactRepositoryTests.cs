using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Setur.Contacts.ContactApi.Repositories;
using Setur.Contacts.Domain.Entities;
using Setur.Contacts.Domain.Enums;

namespace Setur.Contacts.Tests.Repositories;

public class ContactRepositoryTests : TestBase, IDisposable
{
    private readonly ContactRepository _repository;

    public ContactRepositoryTests()
    {
        _repository = GetService<ContactRepository>();
    }

    /// <summary>
    /// Kişi ekleme işleminin veritabanına kaydedilmesini test eder
    /// </summary>
    [Fact]
    public async Task AddAsync_ShouldAddContactToDatabase()
    {
        // Arrange
        var contact = new Contact
        {
            FirstName = "Ahmet",
            LastName = "Yılmaz",
            Company = "Setur"
        };

        // Act
        await _repository.AddAsync(contact);
        await SaveChangesAsync();

        // Assert
        var savedContact = await DbContext.Contacts.FirstOrDefaultAsync(c => c.FirstName == "Ahmet");
        savedContact.Should().NotBeNull();
        savedContact!.FirstName.Should().Be("Ahmet");
        savedContact.LastName.Should().Be("Yılmaz");
        savedContact.Company.Should().Be("Setur");
    }

    /// <summary>
    /// Geçerli ID ile kişi getirme işleminin başarılı olmasını test eder
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnContact()
    {
        // Arrange
        var contact = new Contact { FirstName = "Ahmet", LastName = "Yılmaz", Company = "Setur" };
        await DbContext.Contacts.AddAsync(contact);
        await SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(contact.Id);

        // Assert
        result.Should().NotBeNull();
        result!.FirstName.Should().Be("Ahmet");
        result.LastName.Should().Be("Yılmaz");
        result.Company.Should().Be("Setur");
    }

    /// <summary>
    /// Geçersiz ID ile kişi getirme işleminde null döndürülmesini test eder
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var result = await _repository.GetByIdAsync(invalidId, false);

        // Assert
        result.Should().BeNull();
    }

    /// <summary>
    /// Filtreleme ile kişi getirme işleminin doğru sonuçlar döndürmesini test eder
    /// </summary>
    [Fact]
    public async Task GetWhere_ShouldReturnFilteredContacts()
    {
        // Arrange
        var contacts = new List<Contact>
        {
            new() { FirstName = "Ahmet", LastName = "Yılmaz", Company = "Setur" },
            new() { FirstName = "Mehmet", LastName = "Kaya", Company = "TechCorp" },
            new() { FirstName = "Ali", LastName = "Demir", Company = "Setur" }
        };

        await DbContext.Contacts.AddRangeAsync(contacts);
        await SaveChangesAsync();

        // Act
        var result = await _repository.GetWhere(c => c.Company == "Setur", isTracking: false).ToListAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(c => c.Company == "Setur");
    }

    /// <summary>
    /// Include properties ile ilişkili verilerin dahil edilmesini test eder
    /// </summary>
    [Fact]
    public async Task GetWhere_WithIncludeProperties_ShouldIncludeRelatedData()
    {
        // Arrange
        var contact = new Contact { FirstName = "Ahmet", LastName = "Yılmaz", Company = "Setur" };
        await DbContext.Contacts.AddAsync(contact);
        await SaveChangesAsync();

        var communicationInfo = new CommunicationInfo
        {
            ContactId = contact.Id,
            Type = CommunicationType.Phone,
            Value = "5551234567"
        };

        await DbContext.CommunicationInfos.AddAsync(communicationInfo);
        await SaveChangesAsync();

        // Act
        var result = await _repository.GetWhere(c => c.Id == contact.Id, includeProperties: "CommunicationInfos").FirstOrDefaultAsync();

        // Assert
        result.Should().NotBeNull();
        result!.CommunicationInfos.Should().HaveCount(1);
        result.CommunicationInfos!.First().Value.Should().Be("5551234567");
    }

    /// <summary>
    /// Kişi güncelleme işleminin veritabanında değişiklik yapmasını test eder
    /// </summary>
    [Fact]
    public async Task Update_ShouldUpdateContactInDatabase()
    {
        // Arrange
        var contact = new Contact { FirstName = "Ahmet", LastName = "Yılmaz", Company = "Setur" };
        await DbContext.Contacts.AddAsync(contact);
        await SaveChangesAsync();

        // Act
        contact.FirstName = "Mehmet";
        contact.Company = "TechCorp";
        _repository.Update(contact);
        await SaveChangesAsync();

        // Assert
        var updatedContact = await DbContext.Contacts.FindAsync(contact.Id);
        updatedContact.Should().NotBeNull();
        updatedContact!.FirstName.Should().Be("Mehmet");
        updatedContact.Company.Should().Be("TechCorp");
    }

    /// <summary>
    /// Kişi silme işleminin veritabanından kaydı kaldırmasını test eder
    /// </summary>
    [Fact]
    public async Task Remove_ShouldRemoveContactFromDatabase()
    {
        // Arrange
        var contact = new Contact { FirstName = "Ahmet", LastName = "Yılmaz", Company = "Setur" };
        await DbContext.Contacts.AddAsync(contact);
        await SaveChangesAsync();

        // Act
        _repository.Remove(contact);
        await SaveChangesAsync();

        // Assert
        var deletedContact = await DbContext.Contacts.FindAsync(contact.Id);
        deletedContact.Should().BeNull();
    }

    /// <summary>
    /// Tüm kişileri getirme işleminin başarılı olmasını test eder
    /// </summary>
    [Fact]
    public async Task GetAll_ShouldReturnAllContacts()
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
        var result = await _repository.GetAll().ToListAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(c => c.FirstName == "Ahmet");
        result.Should().Contain(c => c.FirstName == "Mehmet");
    }

    public new void Dispose()
    {
        base.Dispose();
    }
}

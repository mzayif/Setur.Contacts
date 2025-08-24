using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Setur.Contacts.Base.Exceptions;
using Setur.Contacts.ContactApi.Repositories;
using Setur.Contacts.ContactApi.Services;
using Setur.Contacts.Domain.Entities;
using Setur.Contacts.Domain.Enums;
using Setur.Contacts.Domain.Requests;

namespace Setur.Contacts.Tests.Services;

public class CommunicationInfoServiceTests : TestBase, IDisposable
{
    private readonly CommunicationInfoService _communicationInfoService;
    private readonly CommunicationInfoRepository _communicationInfoRepository;
    private readonly ContactRepository _contactRepository;

    public CommunicationInfoServiceTests()
    {
        _communicationInfoRepository = GetService<CommunicationInfoRepository>();
        _contactRepository = GetService<ContactRepository>();
        _communicationInfoService = new CommunicationInfoService(_communicationInfoRepository, _contactRepository);
    }

    /// <summary>
    /// Geçerli verilerle iletişim bilgisi oluşturma işleminin başarılı olmasını test eder
    /// </summary>
    [Fact]
    public async Task CreateCommunicationInfoAsync_WithValidData_ShouldCreateCommunicationInfo()
    {
        // Arrange
        var contact = new Contact { FirstName = "Ahmet", LastName = "Yılmaz", Company = "Setur" };
        await DbContext.Contacts.AddAsync(contact);
        await SaveChangesAsync();

        var request = new CreateCommunicationInfoRequest
        {
            ContactId = contact.Id,
            Type = CommunicationType.Phone,
            Value = "5320001122"
        };

        // Act
        var result = await _communicationInfoService.CreateCommunicationInfoAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Contain("başarıyla oluşturuldu");

        // Verify communication info was saved to database
        var savedCommunicationInfo = await DbContext.CommunicationInfos
            .FirstOrDefaultAsync(c => c.ContactId == contact.Id && c.Type == CommunicationType.Phone);
        savedCommunicationInfo.Should().NotBeNull();
        savedCommunicationInfo!.Value.Should().Be("5320001122");
        savedCommunicationInfo.Type.Should().Be(CommunicationType.Phone);
    }

    /// <summary>
    /// Geçersiz ContactId ile iletişim bilgisi oluşturma işleminde hata fırlatılmasını test eder
    /// </summary>
    [Fact]
    public async Task CreateCommunicationInfoAsync_WithInvalidContactId_ShouldThrowNotFoundException()
    {
        // Arrange
        var invalidContactId = Guid.NewGuid();
        var request = new CreateCommunicationInfoRequest
        {
            ContactId = invalidContactId,
            Type = CommunicationType.Phone,
            Value = "5320001122"
        };

        // Act & Assert
        await _communicationInfoService.Invoking(s => s.CreateCommunicationInfoAsync(request))
            .Should().ThrowAsync<NotFoundException>()
            .WithMessage("NOT_FOUND");
    }

    /// <summary>
    /// Aynı ContactId ve Type ile ikinci bir iletişim bilgisi oluşturma işleminde hata fırlatılmasını test eder
    /// </summary>
    [Fact]
    public async Task CreateCommunicationInfoAsync_WithDuplicateType_ShouldThrowException()
    {
        // Arrange
        var contact = new Contact { FirstName = "Ahmet", LastName = "Yılmaz", Company = "Setur" };
        await DbContext.Contacts.AddAsync(contact);
        await SaveChangesAsync();

        var existingCommunicationInfo = new CommunicationInfo
        {
            ContactId = contact.Id,
            Type = CommunicationType.Phone,
            Value = "5321111111"
        };
        await DbContext.CommunicationInfos.AddAsync(existingCommunicationInfo);
        await SaveChangesAsync();

        var request = new CreateCommunicationInfoRequest
        {
            ContactId = contact.Id,
            Type = CommunicationType.Phone, // Aynı tip
            Value = "5321111111"
        };

        // Act & Assert
        await _communicationInfoService.Invoking(s => s.CreateCommunicationInfoAsync(request))
            .Should().ThrowAsync<Exception>()
            .WithMessage("*zaten mevcut*");
    }

    /// <summary>
    /// Farklı Type ile aynı ContactId'ye iletişim bilgisi ekleme işleminin başarılı olmasını test eder
    /// </summary>
    [Fact]
    public async Task CreateCommunicationInfoAsync_WithDifferentType_ShouldCreateSuccessfully()
    {
        // Arrange
        var contact = new Contact { FirstName = "Ahmet", LastName = "Yılmaz", Company = "Setur" };
        await DbContext.Contacts.AddAsync(contact);
        await SaveChangesAsync();

        var existingCommunicationInfo = new CommunicationInfo
        {
            ContactId = contact.Id,
            Type = CommunicationType.Phone,
            Value = "5551111111"
        };
        await DbContext.CommunicationInfos.AddAsync(existingCommunicationInfo);
        await SaveChangesAsync();

        var request = new CreateCommunicationInfoRequest
        {
            ContactId = contact.Id,
            Type = CommunicationType.Email, // Farklı tip
            Value = "ahmet@setur.com"
        };

        // Act
        var result = await _communicationInfoService.CreateCommunicationInfoAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Contain("başarıyla oluşturuldu");

        // Verify both communication infos exist
        var communicationInfos = await DbContext.CommunicationInfos
            .Where(c => c.ContactId == contact.Id)
            .ToListAsync();
        communicationInfos.Should().HaveCount(2);
        communicationInfos.Should().Contain(c => c.Type == CommunicationType.Phone);
        communicationInfos.Should().Contain(c => c.Type == CommunicationType.Email);
    }

    /// <summary>
    /// ContactId'ye ait tüm iletişim bilgilerini getirme işleminin başarılı olmasını test eder
    /// </summary>
    [Fact]
    public async Task GetCommunicationInfosByContactIdAsync_ShouldReturnAllCommunicationInfos()
    {
        // Arrange
        var contact = new Contact { FirstName = "Ahmet", LastName = "Yılmaz", Company = "Setur" };
        await DbContext.Contacts.AddAsync(contact);
        await SaveChangesAsync();

        var communicationInfos = new List<CommunicationInfo>
        {
            new() { ContactId = contact.Id, Type = CommunicationType.Phone, Value = "5320001122" },
            new() { ContactId = contact.Id, Type = CommunicationType.Email, Value = "ahmet@setur.com" },
            new() { ContactId = contact.Id, Type = CommunicationType.Location, Value = "İstanbul" }
        };
        await DbContext.CommunicationInfos.AddRangeAsync(communicationInfos);
        await SaveChangesAsync();

        // Act
        var result = await _communicationInfoService.GetCommunicationInfosByContactIdAsync(contact.Id);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().HaveCount(3);
        result.Data.Should().Contain(c => c.Type == CommunicationType.Phone && c.Value == "5320001122");
        result.Data.Should().Contain(c => c.Type == CommunicationType.Email && c.Value == "ahmet@setur.com");
        result.Data.Should().Contain(c => c.Type == CommunicationType.Location && c.Value == "İstanbul");
    }

    /// <summary>
    /// Geçersiz ContactId ile iletişim bilgilerini getirme işleminde boş liste döndürmesini test eder
    /// </summary>
    [Fact]
    public async Task GetCommunicationInfosByContactIdAsync_WithInvalidContactId_ShouldReturnEmptyList()
    {
        // Arrange
        var invalidContactId = Guid.NewGuid();

        // Act
        var result = await _communicationInfoService.GetCommunicationInfosByContactIdAsync(invalidContactId);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().BeEmpty();
    }

    /// <summary>
    /// İletişim bilgisi güncelleme işleminin başarılı olmasını test eder
    /// </summary>
    [Fact]
    public async Task UpdateCommunicationInfoAsync_WithValidData_ShouldUpdateCommunicationInfo()
    {
        // Arrange
        var contact = new Contact { FirstName = "Ahmet", LastName = "Yılmaz", Company = "Setur" };
        await DbContext.Contacts.AddAsync(contact);
        await SaveChangesAsync();

        var communicationInfo = new CommunicationInfo
        {
            ContactId = contact.Id,
            Type = CommunicationType.Phone,
            Value = "5551111111"
        };
        await DbContext.CommunicationInfos.AddAsync(communicationInfo);
        await SaveChangesAsync();

        var request = new UpdateCommunicationInfoRequest
        {
            Id = communicationInfo.Id,
            Type = CommunicationType.Phone,
            Value = "5559999999"
        };

        // Act
        var result = await _communicationInfoService.UpdateCommunicationInfoAsync(communicationInfo.Id, request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Contain("başarıyla güncellendi");

        // Verify communication info was updated in database
        var updatedCommunicationInfo = await DbContext.CommunicationInfos.FindAsync(communicationInfo.Id);
        updatedCommunicationInfo.Should().NotBeNull();
        updatedCommunicationInfo!.Value.Should().Be("5559999999");
    }

    /// <summary>
    /// İletişim bilgisi silme işleminin başarılı olmasını test eder
    /// </summary>
    [Fact]
    public async Task DeleteCommunicationInfoAsync_WithValidId_ShouldDeleteCommunicationInfo()
    {
        // Arrange
        var contact = new Contact { FirstName = "Ahmet", LastName = "Yılmaz", Company = "Setur" };
        await DbContext.Contacts.AddAsync(contact);
        await SaveChangesAsync();

        var communicationInfo = new CommunicationInfo
        {
            ContactId = contact.Id,
            Type = CommunicationType.Phone,
            Value = "5320001122"
        };
        await DbContext.CommunicationInfos.AddAsync(communicationInfo);
        await SaveChangesAsync();

        // Act
        var result = await _communicationInfoService.DeleteCommunicationInfoAsync(communicationInfo.Id);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Contain("başarıyla silindi");

        // Verify communication info was deleted from database
        var deletedCommunicationInfo = await DbContext.CommunicationInfos.FindAsync(communicationInfo.Id);
        deletedCommunicationInfo.Should().BeNull();
    }

    public new void Dispose()
    {
        base.Dispose();
    }
}

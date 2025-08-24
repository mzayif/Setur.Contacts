using FluentAssertions;
using Setur.Contacts.Domain.Requests;
using System.Text;
using System.Text.Json;

namespace Setur.Contacts.Tests.Controllers;

public class ContactControllerTests
{
    /// <summary>
    /// CreateContactRequest nesnesinin geçerli olmasını test eder
    /// </summary>
    [Fact]
    public void CreateContactRequest_Should_Be_Valid()
    {
        // Arrange
        var request = new CreateContactRequest
        {
            FirstName = "Test",
            LastName = "User",
            Company = "TestCompany"
        };

        // Act & Assert
        request.Should().NotBeNull();
        request.FirstName.Should().Be("Test");
        request.LastName.Should().Be("User");
        request.Company.Should().Be("TestCompany");
    }

    /// <summary>
    /// UpdateContactRequest nesnesinin geçerli olmasını test eder
    /// </summary>
    [Fact]
    public void UpdateContactRequest_Should_Be_Valid()
    {
        // Arrange
        var request = new UpdateContactRequest
        {
            Id = Guid.NewGuid(),
            FirstName = "Updated",
            LastName = "User",
            Company = "UpdatedCompany"
        };

        // Act & Assert
        request.Should().NotBeNull();
        request.Id.Should().NotBeEmpty();
        request.FirstName.Should().Be("Updated");
        request.LastName.Should().Be("User");
        request.Company.Should().Be("UpdatedCompany");
    }

    /// <summary>
    /// CreateContactRequest nesnesinin JSON serileştirme/deserileştirme işlemlerini test eder
    /// </summary>
    [Fact]
    public void CreateContactRequest_Serialization_Should_Work()
    {
        // Arrange
        var request = new CreateContactRequest
        {
            FirstName = "Test",
            LastName = "User",
            Company = "TestCompany"
        };

        // Act
        var json = JsonSerializer.Serialize(request);
        var deserialized = JsonSerializer.Deserialize<CreateContactRequest>(json);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.FirstName.Should().Be("Test");
        deserialized.LastName.Should().Be("User");
        deserialized.Company.Should().Be("TestCompany");
    }

    /// <summary>
    /// UpdateContactRequest nesnesinin JSON serileştirme/deserileştirme işlemlerini test eder
    /// </summary>
    [Fact]
    public void UpdateContactRequest_Serialization_Should_Work()
    {
        // Arrange
        var request = new UpdateContactRequest
        {
            Id = Guid.NewGuid(),
            FirstName = "Updated",
            LastName = "User",
            Company = "UpdatedCompany"
        };

        // Act
        var json = JsonSerializer.Serialize(request);
        var deserialized = JsonSerializer.Deserialize<UpdateContactRequest>(json);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.Id.Should().Be(request.Id);
        deserialized.FirstName.Should().Be("Updated");
        deserialized.LastName.Should().Be("User");
        deserialized.Company.Should().Be("UpdatedCompany");
    }

    /// <summary>
    /// PagedRequest nesnesinin doğru çalışmasını test eder
    /// </summary>
    [Fact]
    public void PagedRequest_Should_Work_Correctly()
    {
        // Arrange
        var request = new PagedRequest
        {
            PageNumber = 2,
            PageSize = 10
        };

        // Act & Assert
        request.Should().NotBeNull();
        request.PageNumber.Should().Be(2);
        request.PageSize.Should().Be(10);
        request.Skip.Should().Be(10); // (2-1) * 10
        request.Take.Should().Be(10);
    }

    /// <summary>
    /// PagedRequest'te Skip hesaplamasının doğru olmasını test eder
    /// </summary>
    [Theory]
    [InlineData(1, 10, 0)]
    [InlineData(2, 10, 10)]
    [InlineData(3, 5, 10)]
    public void PagedRequest_Skip_Calculation_Should_Be_Correct(int pageNumber, int pageSize, int expectedSkip)
    {
        // Arrange
        var request = new PagedRequest
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        // Act & Assert
        request.Skip.Should().Be(expectedSkip);
    }

    /// <summary>
    /// PagedRequest'te validasyon kurallarının doğru çalışmasını test eder
    /// </summary>
    [Theory]
    [InlineData(0, 10, 1)] // Geçersiz sayfa numarası düzeltilmeli
    [InlineData(-1, 10, 1)] // Geçersiz sayfa numarası düzeltilmeli
    [InlineData(1, 0, 10)] // Geçersiz sayfa boyutu düzeltilmeli
    [InlineData(1, -1, 10)] // Geçersiz sayfa boyutu düzeltilmeli
    [InlineData(1, 101, 100)] // Maksimum sayfa boyutu aşılırsa düzeltilmeli
    public void PagedRequest_Validation_Should_Work(int pageNumber, int pageSize, int expectedPageSize)
    {
        // Arrange
        var request = new PagedRequest
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        // Act & Assert
        request.PageNumber.Should().BeGreaterThan(0);
        request.PageSize.Should().Be(expectedPageSize);
    }
}

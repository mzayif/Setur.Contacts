using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Setur.Contacts.Base.Interfaces;
using Setur.Contacts.ContactApi.Data;
using Setur.Contacts.ContactApi.Repositories;

namespace Setur.Contacts.Tests;

/// <summary>
/// Test sınıfları için base sınıf
/// </summary>
public abstract class TestBase
{
    protected readonly IServiceProvider ServiceProvider;
    protected readonly ContactDbContext DbContext;

    protected TestBase()
    {
        var services = new ServiceCollection();

        // In-Memory Database
        services.AddDbContext<ContactDbContext>(options =>
            options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

        // Repositories
        services.AddScoped<ContactRepository>();
        services.AddScoped<CommunicationInfoRepository>();

        // Services
        services.AddScoped<ILoggerService, TestLoggerService>();

        ServiceProvider = services.BuildServiceProvider();
        DbContext = ServiceProvider.GetRequiredService<ContactDbContext>();
        DbContext.Database.EnsureCreated();
    }

    protected T GetService<T>() where T : class
    {
        return ServiceProvider.GetRequiredService<T>();
    }

    protected async Task SaveChangesAsync()
    {
        await DbContext.SaveChangesAsync();
    }

    protected void Dispose()
    {
        DbContext?.Dispose();
        if (ServiceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}

/// <summary>
/// Test için basit logger servisi
/// </summary>
public class TestLoggerService : ILoggerService
{
    public void LogInformation(string message, params object[] args)
    {
        // Test ortamında log yazmıyoruz
    }

    public void LogWarning(string message, params object[] args)
    {
        // Test ortamında log yazmıyoruz
    }

    public void LogError(string message, Exception? exception = null, params object[] args)
    {
        // Test ortamında log yazmıyoruz
    }

    public void LogDebug(string message, params object[] args)
    {
        // Test ortamında log yazmıyoruz
    }

    public void LogFatal(string message, Exception? exception = null, params object[] args)
    {
        // Test ortamında log yazmıyoruz
    }

    public void Log(LogLevel level, string message, Exception? exception = null, params object[] args)
    {
        // Test ortamında log yazmıyoruz
    }
}

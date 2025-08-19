using Serilog;
using Serilog.Events;
using Setur.Contacts.Base.Interfaces;

namespace Setur.Contacts.Base.Services;

/// <summary>
/// Serilog kullanarak ILoggerService implementasyonu.
/// </summary>
public class SerilogLoggerService : ILoggerService
{
    private readonly ILogger _logger;

    public SerilogLoggerService()
    {
        // Log farmatýný ve daha fazla loglama yollarýný buradan ayarlayabilirsiniz. 
        _logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File("logs/log-.txt", 
                rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();
    }

    public void LogDebug(string message, params object[] args)
    {
        _logger.Debug(message, args);
    }

    public void LogInformation(string message, params object[] args)
    {
        _logger.Information(message, args);
    }

    public void LogWarning(string message, params object[] args)
    {
        _logger.Warning(message, args);
    }

    public void LogError(string message, Exception? exception = null, params object[] args)
    {
        if (exception != null)
        {
            _logger.Error(exception, message, args);
        }
        else
        {
            _logger.Error(message, args);
        }
    }

    public void LogFatal(string message, Exception? exception = null, params object[] args)
    {
        if (exception != null)
        {
            _logger.Fatal(exception, message, args);
        }
        else
        {
            _logger.Fatal(message, args);
        }
    }

    public void Log(LogLevel level, string message, Exception? exception = null, params object[] args)
    {
        var logEventLevel = MapLogLevel(level);
        
        if (exception != null)
        {
            _logger.Write(logEventLevel, exception, message, args);
        }
        else
        {
            _logger.Write(logEventLevel, message, args);
        }
    }

    private static LogEventLevel MapLogLevel(LogLevel level)
    {
        return level switch
        {
            LogLevel.Debug => LogEventLevel.Debug,
            LogLevel.Information => LogEventLevel.Information,
            LogLevel.Warning => LogEventLevel.Warning,
            LogLevel.Error => LogEventLevel.Error,
            LogLevel.Fatal => LogEventLevel.Fatal,
            _ => LogEventLevel.Information
        };
    }
} 
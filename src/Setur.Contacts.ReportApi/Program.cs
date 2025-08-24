using FluentValidation;
using FluentValidation.AspNetCore;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Setur.Contacts.Base.Interfaces;
using Setur.Contacts.Base.Middleware;
using Setur.Contacts.Base.Services;
using Setur.Contacts.Domain.CommonModels;
using Setur.Contacts.MessageBus.Services;
using Setur.Contacts.ReportApi.Data;
using Setur.Contacts.ReportApi.Hubs;
using Setur.Contacts.ReportApi.Repositories;
using Setur.Contacts.ReportApi.Services;
using Setur.Contacts.ReportApi.Validators;

var builder = WebApplication.CreateBuilder(args);

// Configure port
builder.WebHost.UseUrls("http://0.0.0.0:8080");

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add Logger Service
builder.Services.AddSingleton<ILoggerService, SerilogLoggerService>();

// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<CreateReportRequestValidator>();

// Add Mapster
builder.Services.AddMapster();

// Add DbContext
builder.Services.AddDbContext<ReportDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection") ?? 
                      builder.Configuration.GetConnectionString("ReportDb")));

// Add Configuration
builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection("Redis"));
builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection("Kafka"));

// Add HttpClient
builder.Services.AddHttpClient();

// Add Repositories
builder.Services.AddScoped<ReportRepository>();
builder.Services.AddScoped<ReportDetailRepository>();

// Add Redis Cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    var redisSettings = builder.Configuration.GetSection("Redis").Get<RedisSettings>();
    options.Configuration = redisSettings?.ConnectionString ?? "localhost:6379";
    options.InstanceName = redisSettings?.InstanceName ?? "SeturContacts:";
});

// Add SignalR
builder.Services.AddSignalR();

// Add Services
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IReportProcessorService, ReportProcessorService>();
builder.Services.AddScoped<IReportCacheService, RedisReportCacheService>();

// Add Kafka Services
builder.Services.AddSingleton<KafkaAdminService>();
builder.Services.AddScoped<IKafkaProducerService, KafkaProducerService>();

// Add Kafka Consumer as Background Service
builder.Services.AddHostedService<KafkaConsumerService>();

var app = builder.Build();

// Startup log
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("ReportApi uygulaması başlatılıyor...");

// Configure the HTTP request pipeline.
if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Report API V1");
        options.RoutePrefix = string.Empty; // Swagger UI'ı root'ta (/) açar
        options.DocumentTitle = "Setur Reports API";
    });
}

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.UseMiddleware<RequestResponseLoggingMiddleware>();

// Use CORS
app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Map SignalR Hub
app.MapHub<ReportHub>("/reportHub");

logger.LogInformation("ReportApi uygulaması başlatıldı ve dinlemeye hazır!");

app.Run();

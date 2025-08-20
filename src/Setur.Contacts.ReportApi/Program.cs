using FluentValidation;
using FluentValidation.AspNetCore;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Setur.Contacts.Base.Interfaces;
using Setur.Contacts.Base.Middleware;
using Setur.Contacts.Base.Services;
using Setur.Contacts.ReportApi.Data;
using Setur.Contacts.ReportApi.Repositories;
using Setur.Contacts.ReportApi.Services;
using Setur.Contacts.ReportApi.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

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
    options.UseNpgsql(builder.Configuration.GetConnectionString("ReportDb")));

// Add Repositories
builder.Services.AddScoped<ReportRepository>();
builder.Services.AddScoped<ReportDetailRepository>();

// Add Services
builder.Services.AddScoped<IReportService, ReportService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsProduction())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Report API V1");
        options.RoutePrefix = string.Empty; // Swagger UI'ı root'ta (/) açar
        options.DocumentTitle = "Setur Reports API";
    });
}

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.UseMiddleware<RequestResponseLoggingMiddleware>();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

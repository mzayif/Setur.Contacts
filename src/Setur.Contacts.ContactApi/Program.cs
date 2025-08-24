using Microsoft.EntityFrameworkCore;
using Setur.Contacts.Base.Middleware;
using Setur.Contacts.Base.Services;
using Setur.Contacts.Base.Interfaces;
using Setur.Contacts.ContactApi.Data;
using Setur.Contacts.ContactApi.Repositories;
using Setur.Contacts.ContactApi.Services;
using Setur.Contacts.ContactApi.Validators;
using Setur.Contacts.ContactApi.Mappings;
using FluentValidation;
using FluentValidation.AspNetCore;
using Mapster;

var builder = WebApplication.CreateBuilder(args);

// Configure port
builder.WebHost.UseUrls("http://0.0.0.0:8080");

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

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
builder.Services.AddValidatorsFromAssemblyContaining<CreateContactRequestValidator>();

// Add Mapster
builder.Services.AddMapster();

// Add DbContext
builder.Services.AddDbContext<ContactDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection") ?? 
                      builder.Configuration.GetConnectionString("ContactDb")));

// Add Repositories
builder.Services.AddScoped<ContactRepository>();
builder.Services.AddScoped<CommunicationInfoRepository>();

// Add Services
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<ICommunicationInfoService, CommunicationInfoService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsProduction())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Contact API V1");
        options.RoutePrefix = string.Empty; // Swagger UI'ı root'ta (/) açar
        options.DocumentTitle = "Setur Contacts API";
    });
}

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.UseMiddleware<RequestResponseLoggingMiddleware>();

// Use CORS
app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

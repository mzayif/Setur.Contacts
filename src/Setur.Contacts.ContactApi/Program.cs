using Microsoft.EntityFrameworkCore;
using Setur.Contacts.Base.Middleware;
using Setur.Contacts.Base.Services;
using Setur.Contacts.Base.Interfaces;
using Setur.Contacts.ContactApi.Data;
using Setur.Contacts.ContactApi.Repositories;
using Setur.Contacts.ContactApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Add Logger Service
builder.Services.AddSingleton<ILoggerService, SerilogLoggerService>();

// Add DbContext
builder.Services.AddDbContext<ContactDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ContactDb")));

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

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

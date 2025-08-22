using Setur.Contacts.BlazorApp.Services;
using Setur.Contacts.BlazorApp.Services.Abstracts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// HttpClient ve Contact API servisleri
builder.Services.AddHttpClient<IContactService, ContactService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ContactApiBaseUrl"] ?? "http://localhost:7001/");
});

builder.Services.AddHttpClient<IReportService, ReportService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ReportApiBaseUrl"] ?? "http://localhost:7002/");
});

// Report Status Service (Singleton olarak ekle)
builder.Services.AddSingleton<IReportStatusService, ReportStatusService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

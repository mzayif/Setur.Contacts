using Microsoft.JSInterop;
using Setur.Contacts.BlazorApp.Services.Abstracts;

namespace Setur.Contacts.BlazorApp.Services;

/// <summary>
/// Toast notification servisi implementasyonu
/// JavaScript ile toast mesajları gösterir
/// </summary>
public class ToastService : IToastService
{
    private readonly IJSRuntime _jsRuntime;

    public ToastService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public void ShowSuccess(string message, string? title = null)
    {
        try
        {
            _ = _jsRuntime.InvokeVoidAsync("showToast", "success", title ?? "Başarılı", message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ToastService ShowSuccess Error: {ex.Message}");
        }
    }

    public void ShowError(string message, string? title = null)
    {
        try
        {
            _ = _jsRuntime.InvokeVoidAsync("showToast", "error", title ?? "Hata", message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ToastService ShowError Error: {ex.Message}");
        }
    }

    public void ShowWarning(string message, string? title = null)
    {
        try
        {
            _ = _jsRuntime.InvokeVoidAsync("showToast", "warning", title ?? "Uyarı", message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ToastService ShowWarning Error: {ex.Message}");
        }
    }

    public void ShowInfo(string message, string? title = null)
    {
        try
        {
            _ = _jsRuntime.InvokeVoidAsync("showToast", "info", title ?? "Bilgi", message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ToastService ShowInfo Error: {ex.Message}");
        }
    }
}

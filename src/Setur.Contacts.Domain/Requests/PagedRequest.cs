namespace Setur.Contacts.Domain.Requests;

/// <summary>
/// Sayfalama parametreleri
/// </summary>
public class PagedRequest
{
    private int _pageNumber = 1;
    private int _pageSize = 10;

    /// <summary>
    /// Sayfa numarası (varsayılan: 1)
    /// </summary>
    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = value < 1 ? 1 : value;
    }

    /// <summary>
    /// Sayfa başına kayıt sayısı (varsayılan: 10, maksimum: 100)
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value < 1 ? 10 : value > 100 ? 100 : value;
    }

    /// <summary>
    /// Atlanacak kayıt sayısı
    /// </summary>
    public int Skip => (PageNumber - 1) * PageSize;

    /// <summary>
    /// Alınacak kayıt sayısı
    /// </summary>
    public int Take => PageSize;
}

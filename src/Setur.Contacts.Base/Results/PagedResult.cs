namespace Setur.Contacts.Base.Results;

/// <summary>
/// Sayfalanmış veri sonucu
/// </summary>
/// <typeparam name="T">Veri tipi</typeparam>
public class PagedResult<T> : SuccessDataResult<IEnumerable<T>>
{
    /// <summary>
    /// Toplam kayıt sayısı
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Sayfa numarası (1'den başlar)
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Sayfa başına kayıt sayısı
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Toplam sayfa sayısı
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Önceki sayfa var mı?
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Sonraki sayfa var mı?
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    public PagedResult(IEnumerable<T> data, int totalCount, int pageNumber, int pageSize, string message = "Veriler başarıyla getirildi.")
        : base(data, message)
    {
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
    }
}

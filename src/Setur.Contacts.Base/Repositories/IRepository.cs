using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Setur.Contacts.Base.Repositories;

/// <summary>
/// Generic Repository arayüzü. <br/>
/// DB tarafında yapılacak bütün işlemler için kullanılacak olan arayüzdür. <br/>
/// </summary>
/// <typeparam name="TEntity">İşlem yapılacak Entity</typeparam>
/// <typeparam name="TId">Entitye uygun Uniq Id Tipi</typeparam>
public interface IRepository<TEntity, TId> where TEntity : class
{
    DbSet<TEntity> Table { get; }



    Task<bool> AddAsync(TEntity entity);
    Task<bool> AddAsync(List<TEntity> entity);
    bool Update(TEntity entity);
    /// <summary>
    /// Belirtilen kaydı <b>iptal eder</b>.<br/>
    /// Daha önceden iptal edilmiş veya <see cref="CancelableEntityBase{T}"/> base sınıfından türetilmemiş ise Hata fırlatır.
    /// </summary>
    /// <exception cref="TypeAccessException"></exception>
    /// <exception cref="AppBaseException"></exception>
    /// <param name="entity">İptal edilmek istenen kayıt.</param>
    /// <returns></returns>
    bool Cancel(TEntity entity);
    /// <summary>
    /// Belirtilen Id üzerinden kaydı bulur ve <b>iptal eder</b>.<br/>
    /// Bulamaz ise veya daha önceden iptal edilmiş ise Hata fırlatır.
    /// </summary>
    /// <param name="id">Aranacak Kayıt ID</param>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="AppBaseException"></exception>
    /// <returns></returns>
    Task<bool> CancelAsync(TId id);
    /// <summary>
    /// Belirtilen kaydı <b>Aktif eder</b>.<br/>
    /// Zaten Aktif bir kayıt ise veya <see cref="CancelableEntityBase{T}"/> base sınıfından türetilmemiş ise Hata fırlatır.
    /// </summary>
    /// <exception cref="TypeAccessException"></exception>
    /// <exception cref="AppBaseException"></exception>
    /// <param name="entity">Aktif edilmek istenen kayıt.</param>
    /// <returns></returns>
    bool Activate(TEntity entity);
    /// <summary>
    /// Belirtilen Id üzerinden kaydı bulur ve <b>Aktif eder</b>. <br/>
    /// Bulamaz, Zaten Aktif bir kayıt ise veya <see cref="CancelableEntityBase{T}"/> base sınıfından türetilmemiş ise Hata fırlatır
    /// </summary>
    /// <param name="id">Aktif Edilecek Kayıt ID</param>
    /// <exception cref="FileNotFoundException"></exception>
    /// <returns>Entity</returns>
    Task<bool> ActivateAsync(TId id);
    bool Remove(TEntity entity);
    bool Remove(List<TEntity> entity);
    /// <summary>
    /// Belirtilen Id üzerinden kaydı bulur ve <b>Siler</b>. Bulamaz ise Hata fırlatır.
    /// </summary>
    /// <param name="id">Aranacak Kayıt ID</param>
    /// <exception cref="FileNotFoundException"></exception>
    /// <returns>Entity</returns>
    Task<bool> RemoveAsync(TId id);
    Task<int> SaveAsync();






    /// <summary>
    /// Tablodaki tüm kayıtları getirir. Varsayılan olarak Tracking mekanizması tarafından takip edilir.
    /// </summary>
    /// <param name="isTracking"> Kayıtlar  Tracking Mekanizması tarafından takip edilsin mi? Varsayılan olarak True</param>
    /// <returns> Geriye IQueryabel döner. koşul ekleyerek isteğe göre sencron veya Async olarak listelenebilir. </returns>
    IQueryable<TEntity> GetAll(bool isTracking = true);
    /// <summary>
    /// Verilen koşula göre kayıtları listeler. Varsayılan olarak Tracking mekanizması tarafından takip edilir.
    /// </summary>
    /// <param name="method"></param>
    /// <param name="isTracking"></param>
    /// <returns></returns>
    IQueryable<TEntity> GetWhere(Expression<Func<TEntity, bool>>? method = null, bool isTracking = true);
    /// <summary>
    /// Verilen koşula göre kayıtları listeler. Varsayılan olarak Tracking mekanizması tarafından takip edilir.<br/>
    /// String olarak verilen includeProperties ile ilişkilendirilmiş tabloları da dahil eder.
    /// </summary>
    /// <param name="method"></param>
    /// <param name="includeProperties"></param>
    /// <param name="isTracking"></param>
    /// <returns></returns>
    IQueryable<TEntity> GetWhere(Expression<Func<TEntity, bool>>? method = null, string? includeProperties = null, bool isTracking = true);
    /// <summary>
    /// Verilen koşullara göre uyan 1 kayıt döner. Varsayılan olarak Tracking mekanizması tarafından takip edilir.
    /// </summary>
    /// <param name="method"></param>
    /// <param name="isTracking"></param>
    /// <returns></returns>
    Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>> method, bool isTracking = true);
    /// <summary>
    /// Belirtilen Id üzerinden kaydı bulur. Bulamaz ise varsayılan olarak Hata fırlatır.
    /// </summary>
    /// <param name="id">Aranacak Kayıt ID</param>
    /// <param name="throwException">Bulunmaması halinde Hata fırlatılsın mı?</param>
    /// <param name="isTracking">Kayıt Tracking mekanizması tarafından takibe alınsın mı? Varsayılan olarak True</param>
    /// <exception cref="FileNotFoundException"></exception>
    /// <returns>Entity</returns>
    Task<TEntity?> GetByIdAsync(TId id, bool throwException = true, bool isTracking = true);
    /// <summary>
    /// Verilen koşullara göre kaydın var olup olmadığını kontrol eder.
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    bool Exists(Expression<Func<TEntity, bool>> filter);
    /// <summary>
    /// Verilen koşullara göre kaydın var olup olmadığını kontrol eder. Async olarak çalışır.
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filter);
    /// <summary>
    /// Verilen SQL sorgusunu çalıştırır. Sadece sorgu çalıştırır, Kuralsın Update veya İnsert işlemleri için kullanılırı. İşlem sonucunda kaç kaydın etkilendiğini döner.
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    Task<int> ExecuteSqlCommandAsync(string query);
    /// <summary>
    /// Verilen SQL ve parametreleri kullanarak oluşan datayı istenen modelde geri döndürür.
    /// </summary>
    /// <typeparam name="TReturnEntity">SQL sonucunda dönmesi istenen model</typeparam>
    /// <param name="query"> SQL</param>
    /// <param name="parameters"> parametreler </param>
    /// <returns></returns>
    Task<List<TReturnEntity>> ExecuteQueryAsync<TReturnEntity>(string query, params object[] parameters) where TReturnEntity : class, new();
}
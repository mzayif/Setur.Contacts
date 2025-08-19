namespace Setur.Contacts.Base.Interfaces
{
    /// <summary>
    /// Şifre işlemleri için servis interface'i
    /// </summary>
    public interface IPasswordService
    {
        /// <summary>
        /// Şifreyi hash'ler
        /// </summary>
        /// <param name="password">Hash'lenecek şifre</param>
        /// <example> şifre boş ise <see cref="ArgumentException"/> fırlatır </example>
        /// <returns>Hash'lenmiş şifre</returns>
        string HashPassword(string password);

        /// <summary>
        /// Şifre doğrulaması yapar
        /// </summary>
        /// <param name="password">Doğrulanacak şifre</param>
        /// <param name="hashedPassword">Hash'lenmiş şifre</param>
        /// <returns>Doğrulama sonucu</returns>
        bool VerifyPassword(string password, string hashedPassword);

        /// <summary>
        /// Şifre güvenlik kontrolü yapar
        /// </summary>
        /// <param name="password">Kontrol edilecek şifre</param>
        /// <returns>Güvenlik kontrolü sonucu</returns>
        (bool IsValid, string ErrorMessage) ValidatePasswordStrength(string password);
    }
} 
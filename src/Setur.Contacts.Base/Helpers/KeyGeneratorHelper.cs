namespace Setur.Contacts.Base.Helpers
{
    public static class KeyGeneratorHelper
    {
        private static Random random = new Random();

        public static string GetGuid = Guid.CreateVersion7().ToString();


        /// <summary>
        /// Verilen uzunlukta sadece Büyük harflerden ve rakamlardan oluşan rastgele bir string anahtar döner.
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GetStringKey(int length = 8)
        {
            const string chars = "ABCDEFGHIJKLMNOPRSTUVYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// Verilen uzunlukta sadece rakamlardan oluşan rastgele bir string anahtar döner.
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static int GetIntKey(int length = 8)
        {
            if (length > 9) length = 9;

            const string chars = "0123456789";
            return int.Parse(new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray()));
        }
    }
}

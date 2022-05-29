namespace api.Models
{
    public class PasswordCrypt
    {
        private static int difficulty = 10;

        public static String hashPassword(String password)
        {
            String salt = BCrypt.Net.BCrypt.GenerateSalt(difficulty);
            String passwordHash = BCrypt.Net.BCrypt.HashPassword(password, salt);

            return passwordHash;
        }
        public static Boolean checkPassword(String plainText,String hash)
        {
            Boolean verified = false;

            if (hash == null || !hash.StartsWith("$2a$"))
                throw new ArgumentException("invalid hash");

            verified=BCrypt.Net.BCrypt.Verify(plainText, hash);

            return verified;
            
        }
    }
}

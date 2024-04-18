namespace Common
{
    public class AppSettings
    {
        public static string ConnectionStrings { get; private set; }
        public static string JwtSecretKey { get; set; }
        public static int ChroneJobTimeInterval { get; set; }

        public static string GoogleClientId { get; set; }

        public static string GoogleClientSecret { get; set; }
        public static string Mail { get; set; }
        public static string DisplayName { get; set; }
        public static string Password { get; set; }
        public static string Host { get; set; }
        public static int Port { get; set; }
    }
}

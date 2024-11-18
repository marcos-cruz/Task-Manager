namespace Bigai.TaskManager.Infrastructure.Persistence.Settings
{
    public class DbSettings
    {
        public const string DbServer = "DbServer";
        public const string DefaultDbServer = "localhost";
        public const string DbPort = "DbPort";
        public const string DefaultDbPort = "14330";// "1433";
        public const string DbUser = "DbUser";
        public const string DefaultDbUser = "SA"; // apenas para fins de test, não deve ser utilizado SA account!
        public const string DbPassword = "DbPassword";
        public const string DefaultDbPassword = "Pass@word123";
        public const string DbName = "DbName";
        public const string DefaultDbName = "TaskManagerDb";

        public string Host { get; init; } = string.Empty;
        public string Port { get; init; } = string.Empty;
        public string User { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
        public string Database { get; init; } = string.Empty;

        public string ConnectionString => $"Server={Host}, {Port}; Database={Database}; User Id={User}; Password={Password}; TrustServerCertificate=True";
    }
}
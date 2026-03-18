using Microsoft.Data.Sqlite;

namespace Questao5.Infrastructure.Sqlite
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly DatabaseConfig databaseConfig;

        public DbConnectionFactory(DatabaseConfig databaseConfig)
        {
            this.databaseConfig = databaseConfig;
        }

        public SqliteConnection CreateConnection()
        {
            return new SqliteConnection(databaseConfig.Name);
        }
    }
}

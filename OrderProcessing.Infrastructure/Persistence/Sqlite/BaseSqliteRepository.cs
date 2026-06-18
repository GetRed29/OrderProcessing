using Microsoft.Data.Sqlite;

namespace OrderProcessing.Infrastructure.Persistence.Sqlite
{
    public abstract class BaseSqliteRepository
    {
        private readonly string _connectionString;

        protected BaseSqliteRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected async Task<SqliteConnection> GetOpenConnectionAsync()
        {
            var connection = new SqliteConnection(_connectionString);

            await connection.OpenAsync();

            return connection;
        }
    }
}

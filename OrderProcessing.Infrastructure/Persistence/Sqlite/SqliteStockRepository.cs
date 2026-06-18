using Microsoft.Data.Sqlite;
using OrderProcessing.Application.Repositories;

namespace OrderProcessing.Infrastructure.Persistence.Sqlite
{
    public class SqliteStockRepository : IStockRepository
    {
        private readonly string _connectionString;

        public SqliteStockRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> GetStockAsync(Guid productId)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = @"
            SELECT StockQuantity 
            FROM ProductInventory 
            WHERE ProductId = @ProductId;";

            command.Parameters.AddWithValue("@ProductId", productId.ToString());

            var result = await command.ExecuteScalarAsync();

            return result != null ? Convert.ToInt32(result) : 0;
        }

        public async Task ReduceStockAsync(Guid productId, int quantity)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = @"
            UPDATE ProductInventory 
            SET StockQuantity = StockQuantity - @Quantity 
            WHERE ProductId = @ProductId;";

            command.Parameters.AddWithValue("@Quantity", quantity);
            command.Parameters.AddWithValue("@ProductId", productId.ToString());

            await command.ExecuteNonQueryAsync();
        }
    }
}

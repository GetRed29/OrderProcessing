using Microsoft.Data.Sqlite;

namespace OrderProcessing.Infrastructure.Persistence.Sqlite
{
    public interface IDatabaseInitializer
    {
        void Initialize();
    }

    public class SqliteDatabaseInitializer : IDatabaseInitializer
    {
        private readonly string _connectionString;
        public SqliteDatabaseInitializer(string connectionString)
        {
            _connectionString = connectionString;
        }
        public void Initialize()
        {
            SQLitePCL.Batteries.Init();

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText =
            @"
                CREATE TABLE IF NOT EXISTS Customers (
                    Id TEXT PRIMARY KEY,
                    Name TEXT NOT NULL,
                    Email TEXT NOT NULL,
                    Age INTEGER NOT NULL,
                    IsTrusted INTEGER NOT NULL
                );

                CREATE TABLE IF NOT EXISTS Orders (
                    Id TEXT PRIMARY KEY,
                    CustomerId TEXT NOT NULL,
                    Status TEXT NOT NULL,
                    TotalAmount REAL NOT NULL,
                    TotalCurrency TEXT NOT NULL,
                    ShippingStreet TEXT NOT NULL,
                    ShippingCity TEXT NOT NULL,
                    ShippingPostalCode TEXT NOT NULL,
                    ShippingCountry TEXT NOT NULL,
                    FOREIGN KEY (CustomerId) REFERENCES Customers(Id)
                );

                CREATE TABLE IF NOT EXISTS OrderItems (
                    ProductId TEXT NOT NULL,
                    OrderId TEXT NOT NULL,
                    ProductName TEXT NOT NULL,
                    Quantity INTEGER NOT NULL,
                    UnitPrice REAL NOT NULL,
                    Currency TEXT NOT NULL,
                    HasAgeRestrict INTEGER NOT NULL,
                    PRIMARY KEY (ProductId, OrderId),
                    FOREIGN KEY (OrderId) REFERENCES Orders(Id)
                );

                CREATE TABLE IF NOT EXISTS OrderHistory (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    OrderId TEXT NOT NULL,
                    FromState TEXT NOT NULL,
                    ToState TEXT NOT NULL,
                    Timestamp TEXT NOT NULL,
                    FOREIGN KEY (OrderId) REFERENCES Orders(Id)
                );

                CREATE TABLE IF NOT EXISTS ProductInventory (
                       ProductId TEXT PRIMARY KEY,
                       StockQuantity INTEGER NOT NULL
                );
            ";

            command.ExecuteNonQuery();
        }
    }
}

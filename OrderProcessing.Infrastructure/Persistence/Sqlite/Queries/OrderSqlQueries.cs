namespace OrderProcessing.Infrastructure.Persistence.Sqlite.Queries
{
    public static class SqlQueries
    {
        public const string UpsertCustomer = @"
                INSERT INTO Customers (Id, Name, Email, Age, IsTrusted)
                VALUES (@customerId, @name, @email, @age, @isTrusted)
                ON CONFLICT(Id) DO UPDATE SET
                    Name = excluded.Name,
                    Email = excluded.Email,
                    Age = excluded.Age,
                    IsTrusted = excluded.IsTrusted;";

        public const string InsertOrder = @"
                INSERT INTO Orders (Id, CustomerId, Status, ShippingStreet, ShippingCity, ShippingPostalCode, ShippingCountry, TotalAmount, TotalCurrency)
                VALUES (@id, @orderCustomerId, @status, @shippingStreet, @shippingCity, @shippingPostalCode, @shippingCountry, @totalAmount, @totalCurrency);";

        public const string InsertOrderItem = @"
                INSERT INTO OrderItems (ProductId, OrderId, ProductName, Quantity, UnitPrice, Currency, HasAgeRestrict)
                VALUES (@productId, @itemOrderId, @productName, @quantity, @unitPrice, @unitCurrency, @hasAgeRestrict);";

        public const string SelectAllOrdersWithCustomers = @"
               SELECT o.Id AS OrderId, o.Status AS OrderStatus, o.ShippingStreet, o.ShippingCity, o.ShippingPostalCode, o.ShippingCountry, o.TotalAmount, o.TotalCurrency,
                       c.Id AS CustomerId, c.Name AS CustomerName, c.Email AS CustomerEmail, c.Age AS CustomerAge, c.IsTrusted AS CustomerIsTrusted
                FROM Orders o
                INNER JOIN Customers c ON o.CustomerId = c.Id;";

        public const string SelectAllOrderItems = @"
              SELECT OrderId, ProductId, ProductName, Quantity, UnitPrice, Currency, HasAgeRestrict
                FROM OrderItems;";

        public const string SelectOrderByIdWithCustomer = @"
               SELECT o.Id AS OrderId, o.Status AS OrderStatus, o.ShippingStreet AS ShippingStreet, o.ShippingCity AS ShippingCity, o.ShippingPostalCode AS ShippingPostalCode, o.ShippingCountry AS ShippingCountry, o.TotalAmount AS TotalAmount, o.TotalCurrency AS TotalCurrency,
                       c.Id AS CustomerId, c.Name AS CustomerName, c.Email AS CustomerEmail, c.Age AS CustomerAge, c.IsTrusted AS CustomerIsTrusted
                FROM Orders o
                INNER JOIN Customers c ON o.CustomerId = c.Id
                WHERE o.Id = @orderId;";

        public const string SelectOrderItemsByOrderId = @"
              SELECT OrderId, ProductId, ProductName, Quantity, UnitPrice, Currency, HasAgeRestrict
                FROM OrderItems
                WHERE OrderId = @orderId;";

        public const string UpdateOrder = @"
                UPDATE Orders
                SET Status = @status,
                    ShippingStreet = @shippingStreet,
                    ShippingCity = @shippingCity,
                    ShippingPostalCode = @shippingPostalCode,
                    ShippingCountry = @shippingCountry,
                    TotalAmount = @totalAmount,
                    TotalCurrency = @totalCurrency
                WHERE Id = @id;";

        public const string DeleteOrderItemsByOrderId = @"
                DELETE FROM OrderItems
                WHERE OrderId = @orderId;";

        public const string DeleteOrderById = @"
                DELETE FROM Orders WHERE Id = @id;";

        public const string SelectOrderHistoryByOrderId = @"
                SELECT FromState, ToState, Timestamp FROM OrderHistory WHERE OrderId LIKE @orderId";

        public const string InsertOrderHistory = @"
                INSERT INTO OrderHistory (OrderId, FromState, ToState, Timestamp) 
                VALUES (@orderId, @fromState, @toState, @timestamp);";

        public const string SelectOrderStatus = @"
                SELECT OrderStatus FROM Orders WHERE OrderId = @orderId";

        public const string SelectAllOrderHistory = @"
                SELECT OrderId, FromState, ToState, Timestamp FROM OrderHistory";
    }
}

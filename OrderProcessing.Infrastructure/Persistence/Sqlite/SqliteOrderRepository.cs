using OrderProcessing.Application.Repositories;
using OrderProcessing.Domain.Entities;
using OrderProcessing.Domain.States;
using OrderProcessing.Infrastructure.Persistence.Sqlite.Mappers;
using OrderProcessing.Infrastructure.Persistence.Sqlite.Queries;
using System.Reflection;
using System.Reflection.PortableExecutable;

namespace OrderProcessing.Infrastructure.Persistence.Sqlite
{
    public class SqliteOrderRepository : BaseSqliteRepository, IOrderRepository
    {

        public SqliteOrderRepository(string connectionString) : base(connectionString) { }

        public async Task AddAsync(Order order)
        {
            using var connection = await GetOpenConnectionAsync();

            using var transaction = connection.BeginTransaction();

            try
            {
                using var customerCommand = connection.CreateCommand();
                customerCommand.Transaction = transaction;
                customerCommand.CommandText = SqlQueries.UpsertCustomer;
                OrderParameterMapper.MapCustomerParameters(customerCommand, order.Customer);
                await customerCommand.ExecuteNonQueryAsync();

                using var orderCommand = connection.CreateCommand();
                orderCommand.Transaction = transaction;
                orderCommand.CommandText = SqlQueries.InsertOrder;
                OrderParameterMapper.MapOrderParameters(orderCommand, order);
                await orderCommand.ExecuteNonQueryAsync();

                foreach (var item in order.Items)
                {
                    using var itemCommand = connection.CreateCommand();
                    itemCommand.Transaction = transaction;
                    itemCommand.CommandText = SqlQueries.InsertOrderItem;
                    OrderParameterMapper.MapOrderItemParameters(itemCommand, order.Id, item);
                    await itemCommand.ExecuteNonQueryAsync();
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            using var connection = await GetOpenConnectionAsync();

            var historyLookup = new Dictionary<string, List<Order.OrderHistoryEntry>>();
            var orderDictionaries = new Dictionary<string, (OrderId Id, Customer Customer, string Street, string City, string PostalCode, string Country, string Status, decimal TotalAmount, string TotalCurrency)>();
            var itemsGroupedByOrder = new Dictionary<string, List<OrderItem>>();

            using (var historyCmd = connection.CreateCommand())
            {
                historyCmd.CommandText = SqlQueries.SelectAllOrderHistory;

                using (var historyReader = await historyCmd.ExecuteReaderAsync())
                {
                    while (await historyReader.ReadAsync())
                    {
                        string dbOrderId = historyReader.GetString(historyReader.GetOrdinal("OrderId"));

                        var entry = new Order.OrderHistoryEntry(
                            historyReader.GetString(historyReader.GetOrdinal("FromState")),
                            historyReader.GetString(historyReader.GetOrdinal("ToState")),
                            historyReader.GetDateTime(historyReader.GetOrdinal("Timestamp"))
                        );

                        if (!historyLookup.TryGetValue(dbOrderId, out var list))
                        {
                            list = new List<Order.OrderHistoryEntry>();
                            historyLookup[dbOrderId] = list;
                        }
                        list.Add(entry);
                    }
                }
            }

            using var orderCommand = connection.CreateCommand();
            orderCommand.CommandText = SqlQueries.SelectAllOrdersWithCustomers;

            using (var reader = await orderCommand.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var orderIdStr = reader.GetString(reader.GetOrdinal("OrderId"));
                    var customer = OrderDataMapper.MapReaderToCustomer(reader);

                    var orderId = new OrderId(Guid.Parse(orderIdStr));
                    var status = reader.GetString(reader.GetOrdinal("OrderStatus"));

                    var street = reader.GetString(reader.GetOrdinal("ShippingStreet"));
                    var city = reader.GetString(reader.GetOrdinal("ShippingCity"));
                    var postalCode = reader.GetString(reader.GetOrdinal("ShippingPostalCode"));
                    var country = reader.GetString(reader.GetOrdinal("ShippingCountry"));

                    var totalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount"));
                    var totalCurrency = reader.GetString(reader.GetOrdinal("TotalCurrency"));

                    orderDictionaries[orderIdStr] = (orderId, customer, street, city, postalCode, country, status, totalAmount, totalCurrency);
                }
            }

            using var itemCommand = connection.CreateCommand();
            itemCommand.CommandText = SqlQueries.SelectAllOrderItems;
            using (var reader = await itemCommand.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var orderIdLink = reader.GetString(reader.GetOrdinal("OrderId"));
                    var item = OrderDataMapper.MapReaderToOrderItem(reader);

                    if (!itemsGroupedByOrder.ContainsKey(orderIdLink))
                    {
                        itemsGroupedByOrder[orderIdLink] = new List<OrderItem>();
                    }

                    itemsGroupedByOrder[orderIdLink].Add(item);
                }
            }

            var completeOrders = new List<Order>();
            foreach (var kvp in orderDictionaries)
            {
                var orderIdStr = kvp.Key;
                var data = kvp.Value;
                var items = itemsGroupedByOrder.GetValueOrDefault(orderIdStr, new List<OrderItem>());

                var shippingAddress = new Address(data.Street, data.City, data.PostalCode, data.Country);
                var totalMoney = new Money(data.TotalAmount, data.TotalCurrency);

                string orderIdKey = data.Id.Value.ToString();
                var historyEntries = historyLookup.TryGetValue(orderIdKey, out var entries)
                    ? entries
                    : new List<Order.OrderHistoryEntry>();

                var order = OrderDataMapper.MapToDomain(
                    data.Id,
                    data.Customer,
                    shippingAddress,
                    items,
                    totalMoney,
                    data.Status,
                    historyEntries
                );

                completeOrders.Add(order);
            }

            return completeOrders;
        }

        public async Task<Order?> GetByIdAsync(OrderId id)
        {
            using var connection = await GetOpenConnectionAsync();

            var orderIdStr = id.Value.ToString();

            Customer? customer = null;
            string? street = null, city = null, postalCode = null, country = null;
            string? status = null;
            decimal totalAmount = 0;
            string? totalCurrency = null;
            bool orderExists = false;

            using var orderCmd = connection.CreateCommand();
            orderCmd.CommandText = SqlQueries.SelectOrderByIdWithCustomer;
            orderCmd.Parameters.AddWithValue("@orderId", orderIdStr);

            using (var reader = await orderCmd.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    orderExists = true;
                    customer = OrderDataMapper.MapReaderToCustomer(reader);
                    status = reader.GetString(reader.GetOrdinal("OrderStatus"));

                    street = reader.GetString(reader.GetOrdinal("ShippingStreet"));
                    city = reader.GetString(reader.GetOrdinal("ShippingCity"));
                    postalCode = reader.GetString(reader.GetOrdinal("ShippingPostalCode"));
                    country = reader.GetString(reader.GetOrdinal("ShippingCountry"));

                    totalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount"));
                    totalCurrency = reader.GetString(reader.GetOrdinal("TotalCurrency"));
                }

                if (!orderExists)
                {
                    return null;
                }

                var items = new List<OrderItem>();

                using var itemCmd = connection.CreateCommand();
                itemCmd.CommandText = SqlQueries.SelectOrderItemsByOrderId;
                itemCmd.Parameters.AddWithValue("@orderId", orderIdStr);

                using (var itemReader = await itemCmd.ExecuteReaderAsync())
                {
                    while (await itemReader.ReadAsync())
                    {
                        var item = OrderDataMapper.MapReaderToOrderItem(itemReader);
                        items.Add(item);
                    }
                }

                var historyEntries = new List<Order.OrderHistoryEntry>();

                var historyCmd = connection.CreateCommand();
                historyCmd.CommandText = SqlQueries.SelectOrderHistoryByOrderId;
                historyCmd.Parameters.AddWithValue("@orderId", id.Value);

                using (var historyReader = await historyCmd.ExecuteReaderAsync())
                {
                    while (await historyReader.ReadAsync())
                    {
                        historyEntries.Add(new Order.OrderHistoryEntry(
                            historyReader.GetString(historyReader.GetOrdinal("FromState")),
                            historyReader.GetString(historyReader.GetOrdinal("ToState")),
                            historyReader.GetDateTime(historyReader.GetOrdinal("Timestamp"))
                        ));
                    }
                }

                var shippingAddress = new Address(street!, city!, postalCode!, country!);
                var totalMoney = new Money(totalAmount, totalCurrency!);

                return OrderDataMapper.MapToDomain(
                    id,
                    customer!,
                    shippingAddress,
                    items,
                    totalMoney,
                    status!,
                    historyEntries
                );
            }
        }

        public async Task UpdateAsync(Order order)
        {
            using var connection = await GetOpenConnectionAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var lastTransition = order.History.LastOrDefault();
                string oldStateName = lastTransition?.FromState ?? "Pending";
                string newStateName = lastTransition?.ToState ?? order.Status;

                using var orderCommand = connection.CreateCommand();
                orderCommand.Transaction = transaction;
                orderCommand.CommandText = SqlQueries.UpdateOrder;
                OrderParameterMapper.MapOrderParameters(orderCommand, order);
                await orderCommand.ExecuteNonQueryAsync();

                using var deleteItemsCmd = connection.CreateCommand();
                deleteItemsCmd.Transaction = transaction;
                deleteItemsCmd.CommandText = SqlQueries.DeleteOrderItemsByOrderId;
                deleteItemsCmd.Parameters.AddWithValue("@orderId", order.Id.Value.ToString());
                await deleteItemsCmd.ExecuteNonQueryAsync();

                using var historyCmd = connection.CreateCommand();
                historyCmd.Transaction = transaction;
                historyCmd.CommandText = SqlQueries.InsertOrderHistory;

                OrderParameterMapper.MapHistoryParameters(
                    historyCmd,
                    order.Id.Value,
                    oldStateName,
                    newStateName,
                    DateTime.UtcNow
                );


                await historyCmd.ExecuteNonQueryAsync();

                foreach (var item in order.Items)
                {
                    using var itemCommand = connection.CreateCommand();
                    itemCommand.Transaction = transaction;
                    itemCommand.CommandText = SqlQueries.InsertOrderItem;
                    OrderParameterMapper.MapOrderItemParameters(itemCommand, order.Id, item);
                    await itemCommand.ExecuteNonQueryAsync();
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task DeleteAsync(OrderId id)
        {
            // In cazul creeri tabelului OrdersItems, se poate folosi constrangerea ON DELETE CASCADE pentru a sterge automat toate itemele asociate unui order atunci cand orderul este sters.
            // In acest caz, ar fi suficient sa se execute doar comanda de stergere a orderului, iar baza de date se va ocupa de stergerea itemelor asociate.

            using var connection = await GetOpenConnectionAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var orderIdStr = id.Value.ToString();

                using var itemCommand = connection.CreateCommand();
                itemCommand.Transaction = transaction;
                itemCommand.CommandText = SqlQueries.DeleteOrderItemsByOrderId;
                itemCommand.Parameters.AddWithValue("@orderId", orderIdStr);
                await itemCommand.ExecuteNonQueryAsync();

                using var orderCommand = connection.CreateCommand();
                orderCommand.Transaction = transaction;
                orderCommand.CommandText = SqlQueries.DeleteOrderById;
                orderCommand.Parameters.AddWithValue("@id", orderIdStr);
                await orderCommand.ExecuteNonQueryAsync();

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
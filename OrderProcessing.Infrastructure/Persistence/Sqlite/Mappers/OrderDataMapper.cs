using Microsoft.Data.Sqlite;
using OrderProcessing.Domain.Entities;
using OrderProcessing.Domain.States;
using System.Data.Common;
using System.Reflection;
using static OrderProcessing.Domain.Entities.Order;

namespace OrderProcessing.Infrastructure.Persistence.Sqlite.Mappers
{
    public static class OrderDataMapper
    {
        public static Customer MapReaderToCustomer(this SqliteDataReader reader)
        {
            return new Customer
            (
                Guid.Parse(reader.GetString(reader.GetOrdinal("CustomerId"))),
                reader.GetString(reader.GetOrdinal("CustomerName")),
                reader.GetString(reader.GetOrdinal("CustomerEmail")),
                reader.GetInt32(reader.GetOrdinal("CustomerAge")),
                reader.GetInt32(reader.GetOrdinal("CustomerIsTrusted")) == 1
            );
        }

        public static OrderItem MapReaderToOrderItem(this SqliteDataReader reader)
        {
            return new OrderItem
            (
                Guid.Parse(reader.GetString(reader.GetOrdinal("ProductId"))),
                reader.GetString(reader.GetOrdinal("ProductName")),
                reader.GetInt32(reader.GetOrdinal("Quantity")),
                new Money(
                    reader.GetDecimal(reader.GetOrdinal("UnitPrice")),
                    reader.GetString(reader.GetOrdinal("Currency"))
                ),
                reader.GetInt32(reader.GetOrdinal("HasAgeRestrict")) == 1
            );
        }

        public static Order MapToDomain(
            OrderId id,
            Customer customer,
            Address shippingAddress,
            List<OrderItem> items,
            Money totalMoney,
            string statusStr,
            List<Order.OrderHistoryEntry> historyEntries)
        {
            var order = new Order(id, customer, shippingAddress, items, totalMoney);

            string fullClassName = $"OrderProcessing.Domain.States.{statusStr}State";
            Type? stateType = Assembly.GetAssembly(typeof(IOrderState))?.GetType(fullClassName);

            var prop = typeof(Order).GetProperty(nameof(Order.CurrentState));

            if (stateType != null && prop != null)
            {
                var stateInstance = (IOrderState)Activator.CreateInstance(stateType)!;

                prop?.SetValue(order, stateInstance);
            }
            else if (prop != null)
            {
                prop.SetValue(order, new PendingState());
            }

            var historyField = typeof(Order).GetField("_history", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (historyField != null)
            {
                historyField.SetValue(order, historyEntries);
            }

            return order;
        }
    }
}

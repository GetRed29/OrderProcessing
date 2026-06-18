using System.Data.Common;
using Microsoft.Data.Sqlite;
using OrderProcessing.Domain.Entities;

namespace OrderProcessing.Infrastructure.Persistence.Sqlite.Mappers
{
    public static class OrderParameterMapper
    {
        public static void MapCustomerParameters(this SqliteCommand cmd, Customer customer)
        {
                cmd.Parameters.AddWithValue("@customerId", customer.Id.ToString());
                cmd.Parameters.AddWithValue("@name", customer.Name);
                cmd.Parameters.AddWithValue("@email", customer.Email);
                cmd.Parameters.AddWithValue("@age", customer.Age);
                cmd.Parameters.AddWithValue("@isTrusted", customer.IsTrusted ? 1 : 0);
        }

        public static void MapOrderParameters(this SqliteCommand cmd, Order order)
        {
            cmd.Parameters.AddWithValue("@id", order.Id.Value.ToString());
            cmd.Parameters.AddWithValue("@orderCustomerId", order.Customer.Id.ToString());
            cmd.Parameters.AddWithValue("@status", order.Status);

            cmd.Parameters.AddWithValue("@shippingStreet", order.ShippingAddress.Street);
            cmd.Parameters.AddWithValue("@shippingCity", order.ShippingAddress.City);
            cmd.Parameters.AddWithValue("@shippingPostalCode", order.ShippingAddress.PostalCode);
            cmd.Parameters.AddWithValue("@shippingCountry", order.ShippingAddress.Country);

            cmd.Parameters.AddWithValue("@totalAmount", order.TotalAmount.Amount);
            cmd.Parameters.AddWithValue("@totalCurrency", order.TotalAmount.Currency);
        }

        public static void MapOrderItemParameters(this SqliteCommand cmd, OrderId orderId, OrderItem item)
        {
            cmd.Parameters.AddWithValue("@itemOrderId", orderId.Value.ToString());
            cmd.Parameters.AddWithValue("@productId", item.ProductId.ToString());
            cmd.Parameters.AddWithValue("@productName", item.ProductName);
            cmd.Parameters.AddWithValue("@quantity", item.Quantity);

            cmd.Parameters.AddWithValue("@unitPrice", item.UnitPrice.Amount);
            cmd.Parameters.AddWithValue("@unitCurrency", item.UnitPrice.Currency);

            cmd.Parameters.AddWithValue("@hasAgeRestrict", item.HasAgeRestrict ? 1 : 0);
        }

        public static void MapHistoryParameters(this SqliteCommand command, Guid orderId, string fromState, string toState, DateTime timestamp)
        {
            command.Parameters.AddWithValue("@orderId", orderId.ToString());
            command.Parameters.AddWithValue("@fromState", fromState);
            command.Parameters.AddWithValue("@toState", toState);
            command.Parameters.AddWithValue("@timestamp", timestamp);
        }
    }
}

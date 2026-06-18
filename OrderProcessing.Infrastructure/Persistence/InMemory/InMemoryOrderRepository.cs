using OrderProcessing.Domain.Entities;
using System.Collections.Concurrent;

namespace OrderProcessing.Infrastructure.Persistence.InMemory
{
    public class InMemoryOrderRepository
    {
        private readonly ConcurrentDictionary<OrderId, Order> _orders = new();

        public Task AddAsync(Order order)
        {
            _orders.TryAdd(order.Id, order);

            return Task.CompletedTask;
        }

        public Task<IEnumerable<Order>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<Order>>(_orders.Values);

        }

        public Task<Order?> GetByIdAsync(OrderId id)
        {
            _orders.TryGetValue(id, out var order);

            return Task.FromResult(order);
        }

        public Task UpdateAsync(Order order)
        {
            _orders[order.Id] = order;

            return Task.CompletedTask;
        }

        public Task DeleteAsync(OrderId id)
        {
            _orders.TryRemove(id, out _);

            return Task.CompletedTask;
        }
    }
}

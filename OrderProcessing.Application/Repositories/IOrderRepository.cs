using OrderProcessing.Domain.Entities;

namespace OrderProcessing.Application.Repositories
{
    public interface IOrderRepository
    {
        Task AddAsync(Order order);
        Task<IEnumerable<Order>> GetAllAsync();
        Task<Order?> GetByIdAsync(OrderId id);
        Task UpdateAsync(Order order);
        Task DeleteAsync(OrderId id);
    }
}

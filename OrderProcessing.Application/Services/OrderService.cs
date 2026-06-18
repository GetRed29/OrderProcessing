using OrderProcessing.Application.Repositories;
using OrderProcessing.Application.Validation;
using OrderProcessing.Domain.Entities;

namespace OrderProcessing.Application.Services
{
    public class OrderService
    {
        private readonly IOrderRepository _repository;
        private readonly IOrderValidationHandler _validation;

        public OrderService(IOrderRepository repository, IOrderValidationHandler validation)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _validation = validation;
        }

        public async Task<Order?> GetByIdAsync(OrderId id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<ValidationResult> CreateOrderAsync(Order order)
        {
            var context = new ValidationContext(order);

            var validationResult = _validation.Handle(context);
            if (!validationResult.IsValid)
                return validationResult;

            await _repository.AddAsync(order);
            return validationResult;
        }

        public async Task PayOrderAsync(OrderId id)
        {
            var order = await GetRequiredOrderAsync(id);
            order.Pay();
            await _repository.UpdateAsync(order);
        }

        public async Task ProcessOrderAsync(OrderId id)
        {
            var order = await GetRequiredOrderAsync(id);
            order.Process();
            await _repository.UpdateAsync(order);
        }

        public async Task ShipOrderAsync(OrderId id)
        {
            var order = await GetRequiredOrderAsync(id);
            order.Ship();
            await _repository.UpdateAsync(order);
        }

        public async Task DeliverOrderAsync(OrderId id)
        {
            var order = await GetRequiredOrderAsync(id);
            order.Deliver();
            await _repository.UpdateAsync(order);
        }

        public async Task CancelOrderAsync(OrderId id)
        {
            var order = await GetRequiredOrderAsync(id);
            order.Cancel();
            await _repository.UpdateAsync(order);
        }

        private async Task<Order> GetRequiredOrderAsync (OrderId id)
        {
            var order = await _repository.GetByIdAsync(id);
            if (order == null)
                throw new KeyNotFoundException($"Order with id {id.Value} not found.");
            return order;
        }
    }
}

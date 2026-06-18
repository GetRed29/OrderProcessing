using OrderProcessing.Application.Repositories;

namespace OrderProcessing.Application.Validation
{
    public class StockValidationHandler : BaseValidationHandler
    {
        private readonly IStockRepository _stockRepository;

        public StockValidationHandler(IStockRepository stockRepository)
        {
            _stockRepository = stockRepository;
        }

        protected override ValidationResult Validate(ValidationContext context)
        {

            foreach (var item in context.order.Items)
            {
                int currentStock = _stockRepository.GetStockAsync(item.ProductId).GetAwaiter().GetResult();

                if (currentStock < item.Quantity)
                {
                    return ValidationResult.Fail($"Insufficient stock for product {item.ProductId}");
                }
            }
            return ValidationResult.Success();
        }
    }
}

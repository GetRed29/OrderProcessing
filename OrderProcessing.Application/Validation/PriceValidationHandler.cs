namespace OrderProcessing.Application.Validation
{
    public class PriceValidationHandler : BaseValidationHandler
    {
        protected override ValidationResult Validate(ValidationContext context)
        {
            decimal sum = 0;

            foreach (var item in context.order.Items)
            {
                if (item.UnitPrice.Amount <= 0)
                {
                    return ValidationResult.Fail($"Item {item.ProductName} has invalid price.");
                }

                sum += item.UnitPrice.Amount * item.Quantity;
            }

            if (sum != context.order.TotalAmount.Amount)
            {
                return ValidationResult.Fail("Total amount does not match the sum of item prices.");
            }

            return ValidationResult.Success();
        }
    }
}

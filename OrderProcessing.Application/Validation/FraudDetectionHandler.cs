namespace OrderProcessing.Application.Validation
{
    public class FraudDetectionHandler : BaseValidationHandler
    {
        protected override ValidationResult Validate(ValidationContext context)
        {
            if (context.order.TotalAmount.Amount > 10000 && context.order.TotalAmount.Currency == "RON" && context.order.Customer.IsTrusted == false)
            {
                return ValidationResult.Fail("Customer must be trusted for orders above 10,000 RON.");
            }

            if (context.order.Items.Count() > 50)
            {
                return ValidationResult.Fail("Order must not exceed 50 items.");
            }

            return ValidationResult.Success();
        }
    }
}

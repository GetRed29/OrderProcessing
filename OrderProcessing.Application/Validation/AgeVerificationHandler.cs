namespace OrderProcessing.Application.Validation
{
    public class AgeVerificationHandler : BaseValidationHandler
    {
        protected override ValidationResult Validate(ValidationContext context)
        {
            if (context.order.Items.Any(i => i.HasAgeRestrict) && context.order.Customer.Age < 18)
            {
                return ValidationResult.Fail("Customer must be at least 18 years old.");
            }

            return ValidationResult.Success();
        }
    }
}

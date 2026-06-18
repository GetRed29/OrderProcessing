using OrderProcessing.Domain.Entities;

namespace OrderProcessing.Application.Validation
{
    public record ValidationContext(Order order);

    public interface IOrderValidationHandler
    {
        IOrderValidationHandler SetNext(IOrderValidationHandler handler);
        ValidationResult Handle(ValidationContext context);
    }
}

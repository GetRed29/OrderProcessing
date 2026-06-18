namespace OrderProcessing.Domain.Entities
{
    public record OrderItem(Guid ProductId, string ProductName, int Quantity, Money UnitPrice, bool HasAgeRestrict);
}

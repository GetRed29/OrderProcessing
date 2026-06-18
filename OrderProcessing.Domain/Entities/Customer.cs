namespace OrderProcessing.Domain.Entities
{
    public record Customer(Guid Id, string Name, string Email, int Age, bool IsTrusted);
}

namespace OrderProcessing.API.DTOs
{
    public record CreateOrderRequest(
        CustomerDto Customer,
        AddressDto ShippingAddress,
        List<OrderItemDto> Items,
        MoneyDto TotalAmount
    );

    public record CustomerDto(Guid? Id, string Name, string Email, int Age, bool IsTrusted);

    public record AddressDto(string Street, string City, string PostalCode, string Country);

    public record OrderItemDto(Guid? ProductId, string ProductName, int Quantity, decimal UnitPrice, string Currency, bool HasAgeRestrict);
    
    public record MoneyDto(decimal Amount, string Currency);
}

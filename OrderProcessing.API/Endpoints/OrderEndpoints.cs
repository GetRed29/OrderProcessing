using OrderProcessing.Domain.Entities;
using OrderProcessing.Application.Repositories;
using OrderProcessing.Domain.States;
using OrderProcessing.API.DTOs;
using OrderProcessing.Application.Validation;
using AppValidationContext = OrderProcessing.Application.Validation.ValidationContext;

namespace OrderProcessing.API.Endpoints
{
    public static class OrderEndpoints
    {
        public static void MapOrderEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/orders");

            group.MapPost("/", async (CreateOrderRequest _request, IOrderRepository _repository, IOrderValidationHandler _validationHandler) =>
            {
                var customer = new Customer(
                    _request.Customer.Id ?? Guid.NewGuid(),
                    _request.Customer.Name,
                    _request.Customer.Email,
                    _request.Customer.Age,
                    _request.Customer.IsTrusted
                );

                var address = new Address(
                    _request.ShippingAddress.Street,
                    _request.ShippingAddress.City,
                    _request.ShippingAddress.PostalCode,
                    _request.ShippingAddress.Country
                );

                var totalMoney = new Money(
                    _request.TotalAmount.Amount,
                    _request.TotalAmount.Currency
                );

                var domainItems = _request.Items.Select(item => new OrderItem(
                    item.ProductId ?? Guid.NewGuid(),
                    item.ProductName,
                    item.Quantity,
                    new Money(item.UnitPrice, item.Currency),
                    item.HasAgeRestrict
                )).ToList();

                var newOrderId = new OrderId(Guid.NewGuid());
                var order = new Order(newOrderId, customer, address, domainItems, totalMoney);

                var validationResult = _validationHandler.Handle(new AppValidationContext(order));

                if (!validationResult.IsValid)
                {
                    return Results.BadRequest(validationResult.Errors);
                }

                await _repository.AddAsync(order);

                return Results.Created($"/orders/{order.Id.Value}", order);
            });

            group.MapPost("/{id:guid}/pay", async (Guid id, IOrderRepository _repository) =>
            {
                var orderId = new OrderId(id);
                var order = await _repository.GetByIdAsync(orderId);

                if (order is null) return Results.NotFound();

                try
                {
                    order.Pay();
                    await _repository.UpdateAsync(order);

                    return Results.Ok(order);
                }
                catch (InvalidOrderTransitionException ex)
                {
                    return Results.Conflict(new { message = ex.Message });
                }
            });

            group.MapPost("/{id:guid}/process", async (Guid id, IOrderRepository _repository) =>
            {
                var orderId = new OrderId(id);
                var order = await _repository.GetByIdAsync(orderId);

                if (order is null) return Results.NotFound();

                try
                {
                    order.Process();
                    await _repository.UpdateAsync(order);

                    return Results.Ok(order);
                }
                catch (InvalidOrderTransitionException ex)
                {
                    return Results.Conflict(new { message = ex.Message });
                }
            });

            group.MapPost("/{id:guid}/ship", async (Guid id, IOrderRepository _repository) =>
            {
                var orderId = new OrderId(id);
                var order = await _repository.GetByIdAsync(orderId);

                if (order is null) return Results.NotFound();
                
                try
                {
                    order.Ship();
                    await _repository.UpdateAsync(order);

                    return Results.Ok(order);
                }
                catch (InvalidOrderTransitionException ex)
                {
                    return Results.Conflict(new { message = ex.Message });
                }
            });

            group.MapPost("/{id:guid}/deliver", async (Guid id, IOrderRepository _repository) =>
            {
                var orderId = new OrderId(id);
                var order = await _repository.GetByIdAsync(orderId);

                if (order is null) return Results.NotFound();

                try
                {
                    order.Deliver();
                    await _repository.UpdateAsync(order);

                    return Results.Ok(order);
                }
                catch (InvalidOrderTransitionException ex)
                {
                    return Results.Conflict(new { message = ex.Message });
                }
            });

            group.MapPost("/{id:guid}/cancel", async (Guid id, IOrderRepository _repository) =>
            {
                var orderId = new OrderId(id);
                var order = await _repository.GetByIdAsync(orderId);

                if (order is null) return Results.NotFound();

                try
                {
                    order.Cancel();
                    await _repository.UpdateAsync(order);

                    return Results.Ok(order);
                }
                catch (InvalidOrderTransitionException ex)
                {
                    return Results.Conflict(new { message = ex.Message });
                }
            });

            group.MapGet("/", async (IOrderRepository _repository) =>
            {
                var orders = await _repository.GetAllAsync();
                return Results.Ok(orders);
            });

            group.MapGet("/{id:guid}", async (Guid id, IOrderRepository _repository) =>
            {
                var orderId = new OrderId(id);
                var order = await _repository.GetByIdAsync(orderId);

                return order is null ? Results.NotFound() : Results.Ok(order);
            });

            group.MapDelete("/{id:guid}", async (Guid id, IOrderRepository _repository) =>
            {
                var orderId = new OrderId(id);

                var order = await _repository.GetByIdAsync(orderId);

                if (order is null) return Results.NotFound();

                try
                {
                    await _repository.DeleteAsync(orderId);

                    return Results.NoContent();
                }
                catch (InvalidOrderTransitionException ex)
                {
                    return Results.Conflict(new { message = ex.Message });
                }
            });
        }
    }
}

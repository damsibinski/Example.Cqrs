namespace Example.Cqrs.Application.Orders.CreateOrder;

public sealed record CreateOrderRequestDto(Guid ProductId, int Quantity)
{
    public CreateOrderCommand ToCommand() => new(ProductId, Quantity);
}

using Example.Cqrs.Application.Common;
using Example.Cqrs.Domain.Entities;
using Example.Cqrs.Domain.Repositories;
using MediatR;

namespace Example.Cqrs.Application.Orders.CreateOrder;

public sealed record CreateOrderCommand(Guid ProductId, int Quantity)
    : IRequest<Result<CreateOrderResult>>;

public sealed record CreateOrderResult
{
    public required Guid Id { get; init; }
    public required Guid ProductId { get; init; }
    public required int Quantity { get; init; }
    public required decimal TotalPrice { get; init; }
    public required DateTimeOffset ExpectedDeliveryAt { get; init; }
}

public sealed class CreateOrderCommandHandler(
    IProductRepository productRepository,
    IOrderRepository orderRepository,
    TimeProvider timeProvider)
    : IRequestHandler<CreateOrderCommand, Result<CreateOrderResult>>
{
    public async Task<Result<CreateOrderResult>> Handle(
        CreateOrderCommand request,
        CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
        {
            return Error.NotFound(
                ErrorCodes.ProductNotFound,
                $"Product '{request.ProductId}' was not found.");
        }

        var order = Order.Create(product, request.Quantity, timeProvider.GetUtcNow());

        orderRepository.Add(order);
        await orderRepository.SaveChangesAsync(cancellationToken);

        return new CreateOrderResult
        {
            Id = order.Id,
            ProductId = order.ProductId,
            Quantity = order.Quantity,
            TotalPrice = order.TotalPrice,
            ExpectedDeliveryAt = order.ExpectedDeliveryAt,
        };
    }
}

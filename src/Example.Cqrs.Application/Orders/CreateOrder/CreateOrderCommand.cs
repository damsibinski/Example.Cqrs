using Example.Cqrs.Application.Common;
using Example.Cqrs.Domain.Entities;
using Example.Cqrs.Domain.Repositories;
using MediatR;

namespace Example.Cqrs.Application.Orders.CreateOrder;

public sealed record CreateOrderCommand(Guid ProductId, int Quantity)
    : IRequest<Result<CreateOrderResult>>;

public sealed record CreateOrderResult(
    Guid Id,
    Guid ProductId,
    int Quantity,
    decimal TotalPrice,
    DateTimeOffset ExpectedDeliveryAt);

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

        return new CreateOrderResult(
            order.Id,
            order.ProductId,
            order.Quantity,
            order.TotalPrice,
            order.ExpectedDeliveryAt);
    }
}

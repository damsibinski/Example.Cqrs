using Example.Cqrs.Application.Common;
using Example.Cqrs.Domain.Repositories;
using MediatR;

namespace Example.Cqrs.Application.Orders.GetOrder;

public sealed record GetOrderQuery(Guid Id) : IRequest<Result<GetOrderResponseDto>>;

public sealed record GetOrderResponseDto
{
    public required Guid Id { get; init; }
    public required Guid ProductId { get; init; }
    public required int Quantity { get; init; }
    public required decimal TotalPrice { get; init; }
    public required DateTimeOffset ExpectedDeliveryAt { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}

public sealed class GetOrderQueryHandler(IOrderRepository orderRepository)
    : IRequestHandler<GetOrderQuery, Result<GetOrderResponseDto>>
{
    public async Task<Result<GetOrderResponseDto>> Handle(
        GetOrderQuery request,
        CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(request.Id, cancellationToken);
        if (order is null)
        {
            return Error.NotFound(
                ErrorCodes.OrderNotFound,
                $"Order '{request.Id}' was not found.");
        }

        return new GetOrderResponseDto
        {
            Id = order.Id,
            ProductId = order.ProductId,
            Quantity = order.Quantity,
            TotalPrice = order.TotalPrice,
            ExpectedDeliveryAt = order.ExpectedDeliveryAt,
            CreatedAt = order.CreatedAt,
        };
    }
}

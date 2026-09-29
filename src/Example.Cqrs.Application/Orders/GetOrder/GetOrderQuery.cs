using Example.Cqrs.Application.Common;
using Example.Cqrs.Domain.Repositories;
using MediatR;

namespace Example.Cqrs.Application.Orders.GetOrder;

public sealed record GetOrderQuery(Guid Id) : IRequest<Result<GetOrderResponseDto>>;

public sealed record GetOrderResponseDto(
    Guid Id,
    Guid ProductId,
    int Quantity,
    decimal TotalPrice,
    DateTimeOffset ExpectedDeliveryAt,
    DateTimeOffset CreatedAt);

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

        return new GetOrderResponseDto(
            order.Id,
            order.ProductId,
            order.Quantity,
            order.TotalPrice,
            order.ExpectedDeliveryAt,
            order.CreatedAt);
    }
}

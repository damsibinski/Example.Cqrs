using Example.Cqrs.Application.Orders.CreateOrder;
using Example.Cqrs.Application.Orders.GetOrder;
using MediatR;

namespace Example.Cqrs.Api.Endpoints;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders")
            .WithTags("Orders");

        group.MapPost(
                "/",
                async (
                    CreateOrderRequestDto request,
                    IMediator mediator,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result = await mediator.Send(request.ToCommand(), cancellationToken);
                    return result.ToCreatedResult(order => $"/api/orders/{order.Id}");
                })
            .WithName("CreateOrder")
            .Accepts<CreateOrderRequestDto>("application/json")
            .Produces<CreateOrderResult>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .AddValidationFilter<CreateOrderRequestDto>();

        group.MapGet(
                "/{id:guid}",
                async (
                    Guid id,
                    IMediator mediator,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result = await mediator.Send(new GetOrderQuery(id), cancellationToken);
                    return result.ToHttpResult();
                })
            .WithName("GetOrder")
            .Produces<GetOrderResponseDto>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }
}
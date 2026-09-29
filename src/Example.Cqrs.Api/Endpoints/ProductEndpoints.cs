using Example.Cqrs.Application.Products.ListProducts;
using MediatR;

namespace Example.Cqrs.Api.Endpoints;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products")
            .WithTags("Products");

        group.MapGet("/", ListProductsAsync)
            .WithName("ListProducts")
            .Produces<ProductListItemDto[]>();

        return app;
    }

    private static async Task<IResult> ListProductsAsync(
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new ListProductsQuery(), cancellationToken);
        return result.ToHttpResult();
    }
}

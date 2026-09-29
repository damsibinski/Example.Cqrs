using Example.Cqrs.Application.Products.ListProducts;
using MediatR;

namespace Example.Cqrs.Api.Endpoints;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products")
            .WithTags("Products");

        group.MapGet(
                "/",
                async (
                    IMediator mediator,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result = await mediator.Send(new ListProductsQuery(), cancellationToken);
                    return result.ToHttpResult();
                })
            .WithName("ListProducts")
            .Produces<ProductListItemDto[]>();

        return app;
    }
}
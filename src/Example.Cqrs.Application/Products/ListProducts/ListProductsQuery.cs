using Example.Cqrs.Application.Common;
using Example.Cqrs.Domain.Repositories;
using MediatR;

namespace Example.Cqrs.Application.Products.ListProducts;

public sealed record ListProductsQuery : IRequest<Result<ProductListItemDto[]>>;

public sealed record ProductListItemDto(Guid Id, string Name, decimal UnitPrice);

public sealed class ListProductsQueryHandler(IProductRepository productRepository)
    : IRequestHandler<ListProductsQuery, Result<ProductListItemDto[]>>
{
    public async Task<Result<ProductListItemDto[]>> Handle(
        ListProductsQuery request,
        CancellationToken cancellationToken)
    {
        var products = await productRepository.ListAsync(cancellationToken);

        var items = products
            .Select(product => new ProductListItemDto(product.Id, product.Name, product.UnitPrice))
            .ToArray();

        return Result<ProductListItemDto[]>.Success(items);
    }
}

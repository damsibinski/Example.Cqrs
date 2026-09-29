using Example.Cqrs.Application.Products.ListProducts;
using Example.Cqrs.Domain.Entities;
using Example.Cqrs.Domain.Repositories;
using NSubstitute;

namespace Example.Cqrs.Application.UnitTests.Products.ListProducts;

public sealed class ListProductsQueryHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly ListProductsQueryHandler _sut;

    public ListProductsQueryHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _productRepository.ListAsync(Arg.Any<CancellationToken>())
            .Returns([GetProduct()]);

        _sut = new ListProductsQueryHandler(_productRepository);
    }

    [Fact]
    public async Task WhenProductsExist_ShouldReturnMappedItems()
    {
        var product = GetProduct();
        var sutArg = new ListProductsQuery();

        var sutResult = await _sut.Handle(sutArg, CancellationToken.None);

        sutResult.Value.Length.ShouldBe(1);
        sutResult.Value[0].Id.ShouldBe(product.Id);
        sutResult.Value[0].Name.ShouldBe(product.Name);
        sutResult.Value[0].UnitPrice.ShouldBe(product.UnitPrice);
        sutResult.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task WhenNoProductsExist_ShouldReturnEmptyArray()
    {
        _productRepository.ListAsync(Arg.Any<CancellationToken>()).Returns([]);
        var sutArg = new ListProductsQuery();

        var sutResult = await _sut.Handle(sutArg, CancellationToken.None);

        sutResult.Value.ShouldBeEmpty();
        sutResult.IsSuccess.ShouldBeTrue();
    }

    private static Product GetProduct() =>
        Product.Create(
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            "product-name",
            10m);
}

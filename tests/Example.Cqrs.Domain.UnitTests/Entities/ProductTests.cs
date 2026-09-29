using Example.Cqrs.Domain.Entities;

namespace Example.Cqrs.Domain.UnitTests.Entities;

public sealed class ProductTests
{
    [Fact]
    public void WhenAllFieldsAreValid_ShouldCreateProductWithExpectedValues()
    {
        var givenId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var givenName = "  product-name  ";
        var givenUnitPrice = 10m;

        var sutResult = Product.Create(givenId, givenName, givenUnitPrice);

        sutResult.Id.ShouldBe(givenId);
        sutResult.Name.ShouldBe("product-name");
        sutResult.UnitPrice.ShouldBe(givenUnitPrice);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void WhenNameIsInvalid_ShouldThrowArgumentException(string? givenName)
    {
        Should.Throw<ArgumentException>(() =>
            Product.Create(
                Guid.Parse("11111111-1111-1111-1111-111111111111"),
                givenName!,
                10m));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void WhenUnitPriceIsNotPositive_ShouldThrowArgumentOutOfRangeException(decimal givenUnitPrice)
    {
        Should.Throw<ArgumentOutOfRangeException>(() =>
            Product.Create(
                Guid.Parse("11111111-1111-1111-1111-111111111111"),
                "product-name",
                givenUnitPrice));
    }
}

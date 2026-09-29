using Example.Cqrs.Domain.Entities;

namespace Example.Cqrs.Domain.UnitTests.Entities;

public sealed class OrderTests
{
    [Fact]
    public void WhenAllFieldsAreValid_ShouldCreateOrderWithExpectedValues()
    {
        var product = GetProduct();
        var givenCreatedAt = new DateTimeOffset(2026, 6, 13, 10, 0, 0, TimeSpan.Zero);
        var givenQuantity = 2;

        var sutResult = Order.Create(product, givenQuantity, givenCreatedAt);

        sutResult.ProductId.ShouldBe(product.Id);
        sutResult.Quantity.ShouldBe(givenQuantity);
        sutResult.TotalPrice.ShouldBe(product.UnitPrice * givenQuantity);
        sutResult.CreatedAt.ShouldBe(givenCreatedAt);
        sutResult.ExpectedDeliveryAt.ShouldBe(givenCreatedAt.AddDays(Order.DefaultDeliveryDays));
    }

    [Fact]
    public void WhenProductIsNull_ShouldThrowArgumentNullException()
    {
        var givenCreatedAt = new DateTimeOffset(2026, 6, 13, 10, 0, 0, TimeSpan.Zero);

        Should.Throw<ArgumentNullException>(() => Order.Create(null!, 1, givenCreatedAt));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void WhenQuantityIsNotPositive_ShouldThrowArgumentOutOfRangeException(int givenQuantity)
    {
        var product = GetProduct();
        var givenCreatedAt = new DateTimeOffset(2026, 6, 13, 10, 0, 0, TimeSpan.Zero);

        Should.Throw<ArgumentOutOfRangeException>(() => Order.Create(product, givenQuantity, givenCreatedAt));
    }

    private static Product GetProduct() =>
        Product.Create(
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            "product-name",
            10m);
}

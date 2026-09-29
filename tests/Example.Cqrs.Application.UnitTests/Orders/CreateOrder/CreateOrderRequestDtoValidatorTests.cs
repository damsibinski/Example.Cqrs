using Example.Cqrs.Application.Orders.CreateOrder;
using FluentValidation.TestHelper;

namespace Example.Cqrs.Application.UnitTests.Orders.CreateOrder;

public sealed class CreateOrderRequestDtoValidatorTests
{
    private readonly CreateOrderRequestDtoValidator _sut = new();

    [Fact]
    public void WhenAllFieldsAreValid_ShouldNotHaveAnyValidationErrors()
    {
        var sutArg = GetCreateOrderRequestDto();

        var sutResult = _sut.TestValidate(sutArg);

        sutResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void WhenProductIdIsEmpty_ShouldReturnError()
    {
        var sutArg = GetCreateOrderRequestDto() with { ProductId = Guid.Empty };

        var sutResult = _sut.TestValidate(sutArg);

        sutResult.ShouldHaveValidationErrorFor(x => x.ProductId);
        sutResult.Errors.Count.ShouldBe(1);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void WhenQuantityIsNotPositive_ShouldReturnError(int givenQuantity)
    {
        var sutArg = GetCreateOrderRequestDto() with { Quantity = givenQuantity };

        var sutResult = _sut.TestValidate(sutArg);

        sutResult.ShouldHaveValidationErrorFor(x => x.Quantity);
        sutResult.Errors.Count.ShouldBe(1);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void WhenQuantityIsPositive_ShouldNotHaveAnyValidationErrors(int givenQuantity)
    {
        var sutArg = GetCreateOrderRequestDto() with { Quantity = givenQuantity };

        var sutResult = _sut.TestValidate(sutArg);

        sutResult.ShouldNotHaveAnyValidationErrors();
    }

    private static CreateOrderRequestDto GetCreateOrderRequestDto() => new(
        Guid.Parse("11111111-1111-1111-1111-111111111111"),
        2);
}

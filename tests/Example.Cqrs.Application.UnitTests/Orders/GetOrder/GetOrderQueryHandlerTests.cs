using Example.Cqrs.Application.Common;
using Example.Cqrs.Application.Orders.GetOrder;
using Example.Cqrs.Domain.Entities;
using Example.Cqrs.Domain.Repositories;
using NSubstitute;

namespace Example.Cqrs.Application.UnitTests.Orders.GetOrder;

public sealed class GetOrderQueryHandlerTests
{
    private readonly IOrderRepository _orderRepository;
    private readonly GetOrderQueryHandler _sut;

    public GetOrderQueryHandlerTests()
    {
        _orderRepository = Substitute.For<IOrderRepository>();
        _orderRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(GetOrder());

        _sut = new GetOrderQueryHandler(_orderRepository);
    }

    [Fact]
    public async Task WhenAllIsOk_ShouldReturnMappedOrder()
    {
        var order = GetOrder();
        var sutArg = GetGetOrderQuery();

        var sutResult = await _sut.Handle(sutArg, CancellationToken.None);

        sutResult.Value.ProductId.ShouldBe(order.ProductId);
        sutResult.Value.Quantity.ShouldBe(order.Quantity);
        sutResult.Value.TotalPrice.ShouldBe(order.TotalPrice);
        sutResult.Value.ExpectedDeliveryAt.ShouldBe(order.ExpectedDeliveryAt);
        sutResult.Value.CreatedAt.ShouldBe(order.CreatedAt);
        sutResult.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task WhenOrderIsNotFound_ShouldReturnNotFoundError()
    {
        _orderRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Order?)null);
        var sutArg = GetGetOrderQuery();

        var sutResult = await _sut.Handle(sutArg, CancellationToken.None);

        sutResult.Error!.Code.ShouldBe(ErrorCodes.OrderNotFound);
        sutResult.Error.Kind.ShouldBe(ErrorKind.NotFound);
    }

    private static GetOrderQuery GetGetOrderQuery() =>
        new(Guid.Parse("22222222-2222-2222-2222-222222222222"));

    private static Order GetOrder()
    {
        var product = Product.Create(
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            "product-name",
            10m);

        return Order.Create(
            product,
            2,
            new DateTimeOffset(2026, 6, 13, 10, 0, 0, TimeSpan.Zero));
    }
}

using Example.Cqrs.Application.Common;
using Example.Cqrs.Application.Orders.CreateOrder;
using Example.Cqrs.Domain.Entities;
using Example.Cqrs.Domain.Repositories;
using NSubstitute;

namespace Example.Cqrs.Application.UnitTests.Orders.CreateOrder;

public sealed class CreateOrderCommandHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly TimeProvider _timeProvider;
    private readonly CreateOrderCommandHandler _sut;

    public CreateOrderCommandHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _productRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(GetProduct());

        _orderRepository = Substitute.For<IOrderRepository>();
        _orderRepository.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        _timeProvider = Substitute.For<TimeProvider>();
        _timeProvider.GetUtcNow()
            .Returns(new DateTimeOffset(2026, 6, 13, 10, 0, 0, TimeSpan.Zero));

        _sut = new CreateOrderCommandHandler(_productRepository, _orderRepository, _timeProvider);
    }

    [Fact]
    public async Task WhenAllIsOk_ShouldPersistOrderAndReturnResult()
    {
        var product = GetProduct();
        var sutArg = GetCreateOrderCommand();
        var givenCreatedAt = new DateTimeOffset(2026, 6, 13, 10, 0, 0, TimeSpan.Zero);

        var sutResult = await _sut.Handle(sutArg, CancellationToken.None);

        _orderRepository.Received(1).Add(Arg.Is<Order>(order =>
            order.ProductId == product.Id &&
            order.Quantity == sutArg.Quantity &&
            order.TotalPrice == product.UnitPrice * sutArg.Quantity &&
            order.CreatedAt == givenCreatedAt &&
            order.ExpectedDeliveryAt == givenCreatedAt.AddDays(Order.DefaultDeliveryDays)));
        await _orderRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        sutResult.Value.ProductId.ShouldBe(product.Id);
        sutResult.Value.Quantity.ShouldBe(sutArg.Quantity);
        sutResult.Value.TotalPrice.ShouldBe(product.UnitPrice * sutArg.Quantity);
        sutResult.Value.ExpectedDeliveryAt.ShouldBe(givenCreatedAt.AddDays(Order.DefaultDeliveryDays));
        sutResult.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task WhenProductIsNotFound_ShouldReturnNotFoundError()
    {
        _productRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Product?)null);
        var sutArg = GetCreateOrderCommand();

        var sutResult = await _sut.Handle(sutArg, CancellationToken.None);

        sutResult.Error!.Code.ShouldBe(ErrorCodes.ProductNotFound);
        sutResult.Error.Kind.ShouldBe(ErrorKind.NotFound);
        _orderRepository.DidNotReceive().Add(Arg.Any<Order>());
        await _orderRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    private static CreateOrderCommand GetCreateOrderCommand() => new(
        Guid.Parse("11111111-1111-1111-1111-111111111111"),
        2);

    private static Product GetProduct() =>
        Product.Create(
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            "product-name",
            10m);
}

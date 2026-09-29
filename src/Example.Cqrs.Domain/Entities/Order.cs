namespace Example.Cqrs.Domain.Entities;

public sealed class Order
{
    public const int DefaultDeliveryDays = 3;

    public Guid Id { get; private set; }

    public Guid ProductId { get; private set; }

    public int Quantity { get; private set; }

    public decimal TotalPrice { get; private set; }

    public DateTimeOffset ExpectedDeliveryAt { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    private Order()
    {
    }

    public static Order Create(
        Product product,
        int quantity,
        DateTimeOffset createdAt)
    {
        ArgumentNullException.ThrowIfNull(product);

        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");

        return new Order
        {
            ProductId = product.Id,
            Quantity = quantity,
            TotalPrice = product.UnitPrice * quantity,
            ExpectedDeliveryAt = createdAt.AddDays(DefaultDeliveryDays),
            CreatedAt = createdAt
        };
    }
}

namespace Example.Cqrs.Domain.Entities;

public sealed class Product
{
    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public decimal UnitPrice { get; private set; }

    private Product()
    {
    }

    public static Product Create(Guid id, string name, decimal unitPrice)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (unitPrice <= 0)
            throw new ArgumentOutOfRangeException(nameof(unitPrice), "Unit price must be greater than zero.");

        return new Product
        {
            Id = id,
            Name = name.Trim(),
            UnitPrice = unitPrice
        };
    }
}

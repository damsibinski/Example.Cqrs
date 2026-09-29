using Example.Cqrs.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Example.Cqrs.Infrastructure.Persistence.Configurations;

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.Property(order => order.Quantity)
            .IsRequired();

        builder.Property(order => order.TotalPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(order => order.ExpectedDeliveryAt)
            .IsRequired();

        builder.Property(order => order.CreatedAt)
            .IsRequired();

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(order => order.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

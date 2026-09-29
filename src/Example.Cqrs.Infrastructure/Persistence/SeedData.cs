using Example.Cqrs.Domain.Entities;

namespace Example.Cqrs.Infrastructure.Persistence;

public static class SeedData
{
    public static readonly Guid NotebookProductId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid KeyboardProductId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid MouseProductId = Guid.Parse("33333333-3333-3333-3333-333333333333");

    public static void EnsureSeeded(AppDbContext dbContext)
    {
        if (dbContext.Products.Any())
            return;

        dbContext.Products.AddRange(
            Product.Create(NotebookProductId, "Notebook", 3499.00m),
            Product.Create(KeyboardProductId, "Mechanical Keyboard", 449.00m),
            Product.Create(MouseProductId, "Wireless Mouse", 129.00m));

        dbContext.SaveChanges();
    }
}

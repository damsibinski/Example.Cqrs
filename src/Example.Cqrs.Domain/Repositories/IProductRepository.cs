using Example.Cqrs.Domain.Entities;

namespace Example.Cqrs.Domain.Repositories;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<Product>> ListAsync(CancellationToken cancellationToken);
}

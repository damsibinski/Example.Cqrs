using Example.Cqrs.Domain.Entities;
using Example.Cqrs.Domain.Repositories;
using Example.Cqrs.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Example.Cqrs.Infrastructure.Repositories;

public sealed class OrderRepository(AppDbContext dbContext) : IOrderRepository
{
    public Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Orders
            .AsNoTracking()
            .SingleOrDefaultAsync(order => order.Id == id, cancellationToken);

    public void Add(Order order) => dbContext.Orders.Add(order);

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        dbContext.SaveChangesAsync(cancellationToken);
}

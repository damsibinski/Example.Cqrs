using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Example.Cqrs.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddSingleton(TimeProvider.System);

        return services;
    }
}

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Hirotaru.MinimalApiFramework;

public static class EndpointExtensions
{
    /// <summary>
    /// Maps all registered <see cref="IEndpoint"/> instances using their associated <see cref="IEndpointGroup"/>s.
    /// </summary>
    public static IEndpointRouteBuilder MapAllEndpoints(this IEndpointRouteBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        var sp = app.ServiceProvider;

        var endpointGroups = sp.GetServices<IEndpointGroup>()
            .OrderBy(g => g.Name)
            .ToList();

        var endpoints = sp.GetServices<IEndpoint>().ToList();

        ValidateGroups(endpointGroups, endpoints);

        // Group endpoints by GroupName for efficient lookup
        var endpointsByGroup = endpoints
            .Where(e => e.GroupName is not null)
            .GroupBy(e => e.GroupName!)
            .ToDictionary(g => g.Key, g => (IReadOnlyList<IEndpoint>)g.ToList());

        // Map grouped endpoints
        foreach (var group in endpointGroups)
        {
            if (!endpointsByGroup.TryGetValue(group.Name, out var endpointsInGroup) ||
                endpointsInGroup.Count == 0)
            {
                continue;
            }

            group.MapGroup(app, endpointsInGroup);
        }

        // Map ungrouped endpoints at root
        foreach (var endpoint in endpoints.Where(e => e.GroupName is null))
        {
            endpoint.Map(app);
        }

        return app;
    }

    private static void ValidateGroups(
        IReadOnlyCollection<IEndpointGroup> endpointGroups,
        IReadOnlyCollection<IEndpoint> endpoints)
    {
        var duplicateGroupNames = endpointGroups
            .GroupBy(g => g.Name)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateGroupNames.Count != 0)
        {
            throw new InvalidOperationException(
                $"Duplicate endpoint group names detected: {string.Join(", ", duplicateGroupNames)}");
        }

        var knownGroupNames = new HashSet<string>(endpointGroups.Select(g => g.Name));
        var missingGroupReferences = endpoints
            .Where(e => e.GroupName is not null && !knownGroupNames.Contains(e.GroupName))
            .Select(e => e.GroupName)
            .Distinct()
            .ToList();

        if (missingGroupReferences.Count != 0)
        {
            throw new InvalidOperationException(
                $"Endpoints refer to missing groups: {string.Join(", ", missingGroupReferences)}");
        }
    }

    /// <summary>
    /// Scans the assembly containing <typeparamref name="TMarker"/> for implementations of
    /// <see cref="IEndpoint"/> and <see cref="IEndpointGroup"/> and registers them as singletons.
    /// </summary>
    public static IServiceCollection AddEndpointsFromAssemblyContaining<TMarker>(
        this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.Scan(scan => scan
            .FromAssemblyOf<TMarker>()
            .AddClasses(c => c.AssignableTo<IEndpoint>())
                .AsImplementedInterfaces()
                .WithSingletonLifetime()
            .AddClasses(c => c.AssignableTo<IEndpointGroup>())
                .AsImplementedInterfaces()
                .WithSingletonLifetime());

        return services;
    }
}
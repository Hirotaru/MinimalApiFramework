using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Hirotaru.MinimalApiFramework;

public abstract class BaseEndpointGroup : IEndpointGroup
{
    /// <summary>
    /// Logical name of the group. This must be unique across all groups.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Base route pattern for this group (relative).
    /// Defaults to "api", override to change or return null/empty for no prefix.
    /// </summary>
    public virtual string? RoutePrefix => "api";

    public void MapGroup(IEndpointRouteBuilder app, IEnumerable<IEndpoint> endpoints)
    {
        if (app is null) throw new ArgumentNullException(nameof(app));
        if (endpoints is null) throw new ArgumentNullException(nameof(endpoints));

        RouteGroupBuilder group;

        if (string.IsNullOrEmpty(RoutePrefix))
        {
            group = app.MapGroup(string.Empty);
        }
        else
        {
            group = app.MapGroup(RoutePrefix);
        }

        // Allow derived classes to configure policies, filters, etc.
        group = ConfigureGroup(group) ?? group;

        foreach (var endpoint in endpoints)
        {
            endpoint.Map(group);
        }
    }

    /// <summary>
    /// Allows derived groups to customize filters, metadata, policies, etc.
    /// </summary>
    protected abstract RouteGroupBuilder ConfigureGroup(RouteGroupBuilder group);
}
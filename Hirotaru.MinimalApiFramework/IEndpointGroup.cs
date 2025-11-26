using Microsoft.AspNetCore.Routing;

namespace Hirotaru.MinimalApiFramework;

public interface IEndpointGroup
{
    /// <summary>
    /// Logical name of the group. Used for matching endpoints to groups.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Maps the given endpoints as part of this group.
    /// </summary>
    void MapGroup(IEndpointRouteBuilder app, IEnumerable<IEndpoint> endpoints);
}
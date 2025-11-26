using Microsoft.AspNetCore.Routing;

namespace Hirotaru.MinimalApiFramework;

public interface IEndpoint
{
    /// <summary>
    /// Optional name of the group this endpoint belongs to. If null, the endpoint is mapped at the root level.
    /// </summary>
    string? GroupName { get; }

    /// <summary>
    /// Maps this endpoint onto the specified <see cref="IEndpointRouteBuilder"/>.
    /// </summary>
    void Map(IEndpointRouteBuilder app);
}
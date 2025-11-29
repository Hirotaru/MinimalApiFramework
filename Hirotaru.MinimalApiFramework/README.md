# Andrey.MinimalApi

A lightweight helper framework for organizing and discovering Minimal API endpoints in ASP.NET Core applications.  
It provides automatic endpoint registration, endpoint grouping, and assembly scanning using Scrutor.

## Features

- Convention-based endpoint discovery via `IEndpoint` and `IEndpointGroup`
- Automatic registration with `AddEndpointsFromAssemblyContaining<T>()`
- Group-based routing, including optional per-group route prefixes
- Clean separation of concerns between endpoint definition and mapping
- Works seamlessly with ASP.NET Core's `IEndpointRouteBuilder`

## Installation

Install via NuGet:

```bash
dotnet add package Andrey.MinimalApi
````

## Defining Endpoints

Create endpoint classes that implement `IEndpoint`:

```csharp
using Microsoft.AspNetCore.Routing;

public sealed class GetWeatherEndpoint : IEndpoint
{
    public string? GroupName => "Weather";

    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/weather", () =>
        {
            return Results.Ok(new { Temperature = 21, Condition = "Sunny" });
        });
    }
}
```

## Defining Groups

Groups allow related endpoints to share a route prefix, authorization, or other metadata.

```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

public sealed class WeatherGroup : BaseEndpointGroup
{
    public override string Name => "Weather";

    public override string? RoutePrefix => "api/weather";

    protected override RouteGroupBuilder ConfigureGroup(RouteGroupBuilder group)
    {
        return group.RequireAuthorization();
    }
}
```

## Registering Endpoints

In `Program.cs`:

```csharp
using Andrey.MinimalApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsFromAssemblyContaining<Program>();

var app = builder.Build();

app.MapAllEndpoints();

app.Run();
```

All endpoints and endpoint groups in the assembly will automatically be discovered and mapped.

## Requirements

* .NET 8+
* ASP.NET Core Minimal APIs

## License

This project is licensed under the MIT License.
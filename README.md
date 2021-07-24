# Riviera.Kavenegar
Unofficial implementation of Kavenegar API for .NET

[![Nuget Version][nuget-badge]][nuget]
[![Nuget Downloads][nuget-badge-dl]][nuget]

## Installing the NuGet Package
You can install this package by entering the following command into your `Package Manager Console`:

```powershell
Install-Package Riviera.Kavenegar -PreRelease
```

## Startup Configuration
To use `KavenegarService` you need to register it in your `Startup.cs` class.

```csharp
// using Riviera.Kavenegar;

services.AddHttpClient<KavenegarService>();
services.Configure<KavenegarOptions>(o => o.ApiKey = "your-api-key");
```
*Note: Consider using [user secrets][user-secrets] to store the API key.*

## Controller Configuration
After registering the service, you need to add it to your controller.

```csharp
// using Riviera.Kavenegar;

public class HomeController : Controller
{
    private readonly KavenegarService _service;

    public HomeController(KavenegarService service)
    {
        _service = service;
    }
}
```

## Sending a text message
You can send a text message via `SendMessageAsync` method.

```csharp
[Route("send")]
public async Task<IActionResult> Send()
{
    var result = await _service.SendMessageAsync("recipient number", "message", "sender number");

    if (result.IsSuccess)
    {
        return Ok("Message has been successfully sent.");
    }

    // Show an error message
    return Content($"Code: {result.Return.Status}\nMessage: {result.Return.Message}");
}
```

## Checking the status
You can check the text message status via `GetStatusAsync` method.

```csharp
[Route("status")]
public async Task<IActionResult> Status()
{
    var result = await _service.GetStatusAsync(1234567);

    if (result.IsSuccess)
    {
        return Ok(result.Entry.StatusText);
    }

    // Show an error message
    return Content($"Code: {result.Return.Status}\nMessage: {result.Return.Message}");
}
```

## License
This project is licensed under the [MIT License](LICENSE).

[nuget]: https://www.nuget.org/packages/Riviera.Kavenegar
[nuget-badge]: https://img.shields.io/nuget/v/Riviera.Kavenegar.svg?label=Release
[nuget-badge-dl]: https://img.shields.io/nuget/dt/Riviera.Kavenegar?label=Downloads&color=red

[user-secrets]: https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets
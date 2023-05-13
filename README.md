# Rudi.Dev.Azure.Functions.Isolated.Authz

Add Authentication / Authorization Middleware to Azure Functions (running Isolated on .NET 7) when using the built in Azure App Service authentication.

This only supports App Roles (not Scopes).

## Why

- Azure Functions don't have a built in way to access the ClaimsPrincipal
- [Documentation for this](https://learn.microsoft.com/en-us/azure/app-service/configure-authentication-user-identities) has code for In-Process (which this mostly uses)

## Can I use this In-Process?

Don't. [Accept a ClaimsPrincipal](https://learn.microsoft.com/en-us/azure/azure-functions/functions-bindings-http-webhook-trigger?pivots=programming-language-csharp&tabs=python-v2%2Cin-process%2Cfunctionsv2#working-with-client-identities) and use a Filter instead.

## Usage

Add `Rudi.Dev.Azure.Functions.Isolated.Authz` from NuGet.

Add the middleware to your Function worker:
```csharp
.ConfigureFunctionsWorkerDefaults(app =>
    {
        app.AddAuthz(); // <-- Add me
    })
```

Add a good old `[Authorize(..)]` attribute to your functions:
```csharp
[Authorize] // Requires an authenticated user (although don't use this, App Service should be doing this for you)
[Authorize("Required.AppRole")]
[Authorize(new [] { "Required.AppRole", "Another.Required.AppRole" })]
```

## What this actually does

It basically uses the code referenced above from Microsoft to parse the ClientPrincipal, and translate it to a ClaimsPrincipal but with Roles correctly mapped so you can use `principal.IsInRole("")` in your own code.

The Attribute is just a helper so that you don't have to do that.
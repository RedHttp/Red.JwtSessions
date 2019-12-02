# Jwt Sessions for RedHttpServer
### Simple session management middleware for Red using [JWT.Net](https://github.com/jwt-dotnet/jwt)
[![GitHub](https://img.shields.io/github/license/redhttp/red.jwtsessions)](https://github.com/RedHttp/Red.JwtSessions/blob/master/LICENSE.md)
[![Nuget](https://img.shields.io/nuget/v/red.jwtsessions)](https://www.nuget.org/packages/red.jwtsessions/)
[![Nuget](https://img.shields.io/nuget/dt/red.jwtsessions)](https://www.nuget.org/packages/red.jwtsessions/)
![Dependent repos (via libraries.io)](https://img.shields.io/librariesio/dependent-repos/nuget/red.jwtsessions)

### Usage
After installing and referencing this library, the `Red.Response` has the extension method `SendJwtToken(sessionData)`.

`SendJwtToken(sessionData)` creates a new Jwt token using the provided session-data and sends it JSON encoded: `{ "JWT": "Bearer eyJ0eXAiOiJKV1QiL..." }`

### Example
```csharp
class Session
{
    public Guid UserId;
}
...
private static async Task<HandlerType> Auth(Request req, Response res)
{
    if (req.GetJwtData<Session>() == null)
    {
        await res.SendStatus(HttpStatusCode.Unauthorized);
        return HandlerType.Final;
    }
    return HandlerType.Continue;
}

static async Task Main(string[] args)
{
    var server = new RedHttpServer();
    server.Use(new JwtSessions<Session>(new JwtSessionSettings(TimeSpan.FromDays(5), "djklhfbaksdjhfajsdhfasdfhjadsb")));

    var data = new Session
    {
        UserId = Guid.NewGuid()
    };

    server.Get("/login", (req, res) => res.SendJwtToken(data));

    server.Get("/test", Auth, (req, res) =>
    {
        var sessionData = req.GetJwtData<Session>();
        return res.SendString("Hi " + sessionData.UserId);
    });

    await server.RunAsync();
}
```

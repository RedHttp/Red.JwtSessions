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
class MySession 
{
    public string Username;
}
...
server.Use(new JwtSessions<MySession>(new JwtSessionSettings(TimeSpan.FromDays(1), "my secret secret")
{
    ShouldAuthenticate = path => path != "/login" // We allow people to send requests without a valid Authorization to /login, where we can authenticate them
}));

server.Get("/login", async (req, res) =>
{
    var form = await res.GetFormDataAsync();
    if (ValidForm(form) && Authenticate(form["username"], form["password"]))
    {
        await res.SendJwtToken(new MySession {Username = "benny"});
    }
    else 
        await res.SendStatus(HttpStatusCode.BadRequest);
});

// Only authenticated users are allowed to /friends
server.Get("/friends", async (req, res) => 
{
    var session = req.GetData<MySession>();
    var friends = database.GetFriendsOfUser(session.Username);
    await res.SendJson(friends);
});
```

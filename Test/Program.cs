using System;
using System.Net;
using System.Threading.Tasks;
using Red;
using Red.JwtSessions;

namespace Test
{
    class Session
    {
        public Guid UserId;
    }
    
    class Program
    {
        private static async Task Auth(Request req, Response res)
        {
            if (req.GetJwtData<Session>() == null)
            {
                await res.SendStatus(HttpStatusCode.Unauthorized);
            }
        }
        
        static async Task Main(string[] args)
        {
            var server = new RedHttpServer();
            server.RespondWithExceptionDetails = true;
            server.Use(new JwtSessions<Session>(new JwtSessionSettings(TimeSpan.FromDays(5), "djklhfbaksdjhfajsdhfasdfhjadsb")));

            var data = new Session
            {
                UserId = Guid.NewGuid()
            };
            
            server.Get("/login", async (req, res) =>
            {
                await res.SendJwtToken(data);
            });
            
            server.Get("/test", Auth, async (req, res) =>
            {
                var sessionData = req.GetJwtData<Session>();
                await res.SendString("Hi " + data.UserId);
            });

            await server.RunAsync();
        }
    }
}
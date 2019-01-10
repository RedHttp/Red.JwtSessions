using System.Threading.Tasks;

namespace Red.JwtSessions
{
    public static class JwtSessionExtensions
    {
        /// <summary>
        ///     Creates a Jwt token and sends it wrapped in an object: { "JWT": "Bearer eyJ0eXAiOiJKV1QiL..." }
        /// </summary>
        /// <param name="response"></param>
        /// <param name="sessionData"></param>
        public static async Task SendJwtToken<TSession>(this Response response, TSession sessionData)
        {
            var manager = response.ServerPlugins.Get<JwtSessions<TSession>>();
            var auth = manager.NewSession(sessionData);
            await response.SendJson(new JwtContainer { JWT = auth });
        }

        /// <summary>
        ///     For decoding and retrieving the JWT from the Authorization header.
        ///     Will return null if no JWT was found.
        /// </summary>
        /// <param name="req">This request</param>
        /// <typeparam name="TSession">The type of the object stored in the JWT</typeparam>
        /// <returns>The decoded data parameter from the JWT</returns>
        public static TSession GetJwtData<TSession>(this Request req)
        {
            return req.GetData<TSession>();
        }
    }

    class JwtContainer
    {
        public string JWT;
    }
}
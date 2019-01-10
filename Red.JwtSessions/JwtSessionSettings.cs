using System;
using JWT.Algorithms;

namespace Red.JwtSessions
{
    public class JwtSessionSettings
    {
        /// <summary>
        ///     Algorithm used by Jwt-library. Defaults to HMAC-SHA256-A1
        /// </summary>
        public IJwtAlgorithm Algorithm { get; set; } = new HMACSHA256Algorithm();
        
        public TimeSpan SessionLength { get; }
        public string Secret { get; }
        
      
        public JwtSessionSettings(TimeSpan sessionLength, string secret)
        {
            SessionLength = sessionLength;
            Secret = secret;
        }
    }
}
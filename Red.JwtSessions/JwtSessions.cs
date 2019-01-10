﻿using System;
using System.Threading.Tasks;
using JWT;
using JWT.Builder;
using Newtonsoft.Json;
using Red.Interfaces;

namespace Red.JwtSessions
{
    public class JwtSessions<TSession> : IRedMiddleware, IRedWebSocketMiddleware
    {
        /// <summary>
        ///     Constructor for JwtSessions Middleware
        /// </summary>
        /// <param name="settings">Settings object</param>
        public JwtSessions(JwtSessionSettings settings)
        {
            _settings = settings;
        }
        
 
        private readonly JwtSessionSettings _settings;

        /// <summary>
        ///     Do not invoke. Is invoked by the server when it starts. 
        /// </summary>
        public void Initialize(RedHttpServer server)
        {
            server.Plugins.Register(this);
        }

        /// <summary>
        ///     Do not invoke. Is invoked by the server with every websocket request
        /// </summary>
        public Task Process(Request req, WebSocketDialog wsd, Response res) => ProcessInternal(req, res);

        /// <summary>
        ///     Do not invoke. Is invoked by the server with every request
        /// </summary>
        public Task Process(Request req, Response res) => ProcessInternal(req, res);

        private Task ProcessInternal(Request req, Response res)
        {
            return Task.Run(() =>
            {
                string token = null;
                string auth = req.Headers["Authorization"];

                if (string.IsNullOrEmpty(auth))
                {
                    return;
                }

                if (auth.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    token = auth.Substring("Bearer ".Length).Trim();
                }

                if (!string.IsNullOrEmpty(token) && TryAuthenticateToken(token, out var session))
                {
                    req.SetData(session.Data);
                }
            });
        }
        
        private bool TryAuthenticateToken(string authorization, out JwtSession data)
        {
            try
            {
                var json = new JwtBuilder()
                    .WithSecret(_settings.Secret)
                    .WithAlgorithm(_settings.Algorithm)
                    .MustVerifySignature()
                    .Decode(authorization);
                data = JsonConvert.DeserializeObject<JwtSession>(json);
                return true;
            }
            catch (Exception)
            {
                data = null;
                return false;
            }
        }
        
        internal string NewSession(TSession sessionData)
        {
            var token = new JwtBuilder()
                .WithSecret(_settings.Secret)
                .WithAlgorithm(_settings.Algorithm)
                .AddClaim("exp", DateTimeOffset.UtcNow.Add(_settings.SessionLength).ToUnixTimeSeconds())
                .AddClaim("data", sessionData )
                .Build();
            
            return $"Bearer {token}";
        }

        private class JwtSession
        {
            public TSession Data;
        }

        
    }
}
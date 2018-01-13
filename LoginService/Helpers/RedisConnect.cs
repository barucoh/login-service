using System;
using StackExchange.Redis;
using Microsoft.Extensions.Options;

namespace LoginService.Helpers
{
    public interface IRedisConnectionFactory
    {
        ConnectionMultiplexer Connection();
    }

    public class RedisConnectionFactory : IRedisConnectionFactory
    {
        private readonly Lazy<ConnectionMultiplexer> _connection;

        private readonly IOptions<ConfigurationOptions> redis;

        public RedisConnectionFactory(IOptions<ConfigurationOptions> redis)
        {
            this._connection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect("portal282-7.bmix-lon-yp-6194d336-1971-40ed-9ef6-4b7774867f58.ohadbaruch1-gmail-com.composedb.com:22743,password=HIOQEQQBKQOQRLXB,Ssl=true"));
        }

        public ConnectionMultiplexer Connection()
        {
            return this._connection.Value;
        }
    }
}

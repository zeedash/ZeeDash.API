namespace ZeeDash.API.SiloServer;

using StackExchange.Redis;

public static class RedisExtensions {

    /// <summary>
    /// Add redis service to DI
    /// </summary>
    public static IServiceCollection AddRedisServices(this IServiceCollection services, IConfiguration configuration) {
        services.AddStackExchangeRedisCache(options => options.Configuration = configuration.GetConnectionString("Redis"));

        var redisConn = configuration.GetSection("Clustering").GetValue<string>("ConnectionString");
        if (!string.IsNullOrEmpty(redisConn)) {
            var cm = ConnectionMultiplexer.Connect(redisConn);
            services.AddSingleton<IConnectionMultiplexer>(cm);
        }

        return services;
    }
}

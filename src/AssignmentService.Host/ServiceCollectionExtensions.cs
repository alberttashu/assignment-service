namespace AssignmentService.Host
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using MongoDB.Driver;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAssignmentService(this IServiceCollection services,
            IConfiguration configuration)
        {
            var config = new Config();
            configuration.GetSection("application").Bind(config);
            var connection = configuration.GetValue<string>("environment:mongo:connectionString");
            services.AddMongoDb(connection, config.SeedingEnabled, config.SeedFilePath);

            services.AddSingleton<AssignmentService>();
            services.AddControllers();
            
            return services;
        }

        public static IServiceCollection AddMongoDb(this IServiceCollection services, string connectionString,
            bool seedingEnabled = false, string seedFilePath = null)
        {
            var client = new MongoClient(connectionString);
            var db = client.GetDatabase(Constants.AssignmentsDbName);
            if (seedingEnabled && seedFilePath != null)
            {
                DataSeeder.SeedAsync(db, seedFilePath)
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();
            }

            services.AddSingleton(db);
            return services;
        }
    }
}
namespace AssignmentService.Host
{
    using System.Drawing;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Diagnostics.HealthChecks;

    public static class Constants
    {
        public const string AssignmentsDbName = "Assignments";
    }

    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            var connection = _configuration.GetValue<string>("environment:mongo:connectionString");
            var client = new MongoDB.Driver.MongoClient(connection);
            var db = client.GetDatabase(Constants.AssignmentsDbName);
            services.AddSingleton(db);
            services.AddHealthChecks()
                .AddMongoDb(connection, "assignments-db", HealthStatus.Unhealthy);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHealthChecks("/health");
        }
    }
}
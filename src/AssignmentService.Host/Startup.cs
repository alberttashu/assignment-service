namespace AssignmentService.Host
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Diagnostics.HealthChecks;

    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAssignmentService(_configuration);

            var connection = _configuration.GetValue<string>("environment:mongo:connectionString");
            services.AddHealthChecks()
                .AddMongoDb(connection, "assignments-db", HealthStatus.Unhealthy);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHealthChecks("/health");
            app.UseRouting();
            app.UseEndpoints(config =>
            {
                config.MapControllers();
            });
        }
    }
}
using Infrastructure;
using Template.Persistence.Common;
using Template.Application;
using Template.Persistence;

namespace Template.API
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration) => _configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = _configuration.GetConnectionString("PostgresConnection");
            services.AddFluentMigrator(
                connectionString,
                typeof(SqlMigration).Assembly);

            ConfigurationObjectBuilder configObjectBuilder = new(_configuration);
            var configurationObject = configObjectBuilder.Configure();

            services.AddSingleton(configurationObject);
            PersistenceConfiguration.AddServices(services);
            ApplicationConfiguration.AddServices(services);
        }



        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseCors("MyAllowedOrigins");
            }

            app.UseHealthChecks("/api/health");
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();

            app.UseAuthorization();
            app.UseEndpoints(
            endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

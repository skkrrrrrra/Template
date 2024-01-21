using Microsoft.Extensions.DependencyInjection;

namespace Template.Persistence
{
    public class PersistenceConfiguration
    {
        public static void AddServices(IServiceCollection services)
        {
            services.AddScoped<MainDbContext>();
        }
    }
}

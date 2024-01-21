using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Template.Persistence.Context;

namespace Template.Persistence
{
    public class PersistenceConfiguration
    {
        public static void AddServices(IServiceCollection services, string connectionString)
        {
            services.AddDbContext<MainDbContext>(options => options.UseNpgsql(connectionString));
        }
    }
}

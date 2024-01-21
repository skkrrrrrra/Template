using Microsoft.Extensions.DependencyInjection;
using Template.Application.Services;
using Template.Application.Services.Interfaces;

namespace Template.Application
{
    public class ApplicationConfiguration
    {
        public static void AddServices(IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
        }
    }
}

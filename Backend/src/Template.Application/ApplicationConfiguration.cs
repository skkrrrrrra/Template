using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Template.Application.Services;
using Template.Application.Services.Audit;
using Template.Application.Services.Base;
using Template.Application.Services.Interfaces;
using Template.Persistence;

namespace Template.Application
{
    public class ApplicationConfiguration
    {
        public static void AddServices(IServiceCollection services)
        {
            services.AddScoped<IAuditUserProvider, AuditUserProvider>();
            services.AddScoped<Service<User>>();
            services.AddScoped<IAuthService, AuthService>();
        }
    }
}

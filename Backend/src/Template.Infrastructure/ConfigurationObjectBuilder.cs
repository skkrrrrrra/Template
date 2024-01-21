using Microsoft.Extensions.Configuration;
using Template.Domain.Configurations;

namespace Infrastructure
{
    public class ConfigurationObjectBuilder
    {
        private readonly IConfiguration _configuration;
        public ConfigurationObjectBuilder(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ConfigurationObject Configure()
        {
            return new()
            {
                Token = _configuration.GetRequiredSection("Token").Value
            };
        }
    }
}

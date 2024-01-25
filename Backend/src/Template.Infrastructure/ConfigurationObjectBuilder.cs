using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
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
                ConnectionString = _configuration.GetConnectionString("PostgresConnection"),
                Jwt = new()
                {
                    Issuer = _configuration["Jwt:Issuer"],
                    Audience = _configuration["Jwt:Audience"],
                    Key = _configuration["Jwt:Key"]
                }
            };
        }
    }
}

namespace Template.Domain.Configurations
{
    public class ConfigurationObject
    {
        public required string ConnectionString { get; init; }
        public required Jwt Jwt { get; init; }
    }

    public class Jwt
    {
        public required string Issuer { get; init; }
        public required string Audience { get; init; }
        public required string Key { get; init; }
    }
}

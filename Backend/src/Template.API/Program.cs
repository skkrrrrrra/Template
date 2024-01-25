using FluentMigrator.Runner;
using NLog.Extensions.Logging;
using NLog.Web;

namespace Template.API
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            NLogBuilder.ConfigureNLog("nlog.config");
            CreateHostBuilder(args)
            .Build()
            .RunWithMigrate(args);
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host
                .CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.AddNLog();
                })
                .UseNLog()
                .ConfigureWebHostDefaults(builder =>
                {
                    builder.UseStartup<Startup>();
                });
        }

        private static void RunWithMigrate(this IHost host, string[] args)
        {
            if (args.Length > 0 && args[0].Equals("migrate", StringComparison.InvariantCultureIgnoreCase))
            {
                using var scope = host.Services.CreateScope();
                var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

                runner.MigrateUp();
            }
            else
            {
                host.Run();
            }
        }
    }
}
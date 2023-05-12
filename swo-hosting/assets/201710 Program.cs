using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pyra.Foundation.ServiceFabric.Web;
using Serilog;
using SoftwareOne.ResourceManager.Configuration;

namespace SoftwareOne.ResourceManager.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var configurationBuilder = new DefaultConfigurationBuilder(args);
                Log.Logger = configurationBuilder.BuildLogger();

                using (var loggerFactory = new LoggerFactory().AddSerilog())
                {
                    try
                    {
                        var config = configurationBuilder.BuildConfiguration();
                        var host = new HybridWebHost(config.GetSection("Hosting"), loggerFactory, $"{config["CodeName"]}Type");

                        host.Configure(builder => builder
                            .ConfigureServices(services =>
                            {
                                services.AddSingleton(config);
                                services.AddSingleton(_ => Log.Logger);
                                services.AddSingleton(loggerFactory);
                            })
                            .UseConfiguration(config)
                            .UseLoggerFactory(loggerFactory)
                            .UseContentRoot(Directory.GetCurrentDirectory())
                            .UseStartup<Startup>());

                        host.Run();
                    }
                    catch (Exception e)
                    {
                        var logger = loggerFactory.CreateLogger(typeof(Program));
                        logger.LogError(0, e, e.Message);
                        logger.LogCritical(1, e, "Application exited with error.");
                        throw;
                    }
                }
            }
            catch (Exception e)
            {
                EmergencyLogger.Create().Error(e, "Application exited with error: " + e.Message);
                throw;
            }
        }
    }
}

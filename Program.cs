using System;
using System.IO;
using Google.Analytics.Data.V1Beta;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Google.Analytics.Data.Export.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (!File.Exists("credentials.json"))
                throw new Exception("credentials.json must be present!");
            if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("GAD_PROPERTY_ID")))
                throw new Exception("GAD_PROPERTY_ID environment variable must be set!");

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(builder => { builder.UseStartup<Startup>();})
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton(new BetaAnalyticsDataClientBuilder {CredentialsPath = "credentials.json"}
                        .Build());
                    services.AddHostedService<Worker>();
                });
    }
}
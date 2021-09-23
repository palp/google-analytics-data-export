using System;
using System.Threading;
using System.Threading.Tasks;
using Google.Analytics.Data.V1Beta;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Prometheus;

namespace Google.Analytics.Data.Export.Service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly BetaAnalyticsDataClient _client;
        private static readonly string propertyId = System.Environment.GetEnvironmentVariable("GAD_PROPERTY_ID");
        private static readonly Gauge ActiveUsers = Metrics.CreateGauge("gad_activeusers_1m", "Number of active users in the past minute");

        private static readonly Gauge ScreenPageViews = Metrics.CreateGauge("gad_screenpageviews_1m",
            "Number of screen/page views in the past minute");

        public Worker(ILogger<Worker> logger, BetaAnalyticsDataClient client)
        {
            _logger = logger;
            _client = client;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                var request = new RunRealtimeReportRequest
                {
                    Property = $"properties/{propertyId}",
                    Dimensions = {new Dimension {Name = "minutesAgo"}},
                    Metrics = {new Metric {Name = "activeUsers"}, new Metric {Name = "screenPageViews"}},
                    MinuteRanges = { new MinuteRange { Name = "now", EndMinutesAgo = 1, StartMinutesAgo = 1}}
                };
                var response = await _client.RunRealtimeReportAsync(request, stoppingToken);

                foreach (var row in response.Rows)
                {
                    ActiveUsers.Set(double.Parse(row.MetricValues[0].Value));
                    ScreenPageViews.Set(double.Parse(row.MetricValues[1].Value));
                }
                await Task.Delay(20000, stoppingToken);
            }
        }
    }
}
using Microsoft.AspNetCore.Builder;
using Prometheus;

namespace Google.Analytics.Data.Export.Service
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UseMetricServer();
        }
    }
}
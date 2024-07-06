using System;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Toolkit.Services
{
    public static class ApmNewRelic
    {
        public static void AddApmNewRelic(this IServiceCollection services, string endPoint, string apiKey, string serviceName)
        {
            services.AddOpenTelemetry()
                  .ConfigureResource(resource => resource.AddService(serviceName))
                  .WithTracing(tracing => tracing
                      .AddAspNetCoreInstrumentation()
                      .AddEntityFrameworkCoreInstrumentation()
                      .AddHttpClientInstrumentation()
                      .AddOtlpExporter(options =>
                      {
                          options.Endpoint = new Uri(endPoint);
                          options.Headers = $"api-key= {apiKey}";
                      }))
                  .WithMetrics(metrics => metrics
                      .AddAspNetCoreInstrumentation()
                      .AddOtlpExporter(options =>
                      {
                          options.Endpoint = new Uri(endPoint);
                          options.Headers = $"api-key= {apiKey}";

                      }));
        }
    }
}

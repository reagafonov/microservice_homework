using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;

namespace VParkingNotification;

internal sealed class  HealthCheck(LiveProbe liveProbe) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        string probe = JsonConvert.SerializeObject(liveProbe);
        return Task.FromResult(HealthCheckResult.Healthy(probe));
    }
}
// Copyright The OpenTelemetry Authors
// SPDX-License-Identifier: Apache-2.0

using System.Diagnostics;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace OpenTelemetry.Instrumentation.HealthCheck;

public sealed class OpenTelemetryPublisher : IHealthCheckPublisher
{
    private readonly Dictionary<string, HealthStatus> currentStates = [];

    public async Task PublishAsync(HealthReport report, CancellationToken cancellationToken)
    {
        foreach (var entry in report.Entries)
        {
            var activity = HealthCheckInstrumentationActivitySource.ActivitySource.StartActivity(
                    $"{HealthCheckInstrumentationActivitySource.ActivitySourceName}.Process",
                    ActivityKind.Internal);
            bool changedState = true;
            if (this.currentStates.TryGetValue(entry.Key, out HealthStatus previousState) &&
                previousState == entry.Value.Status)
            {
                changedState = false;
            }

            if (changedState)
            {
                this.currentStates[entry.Key] = entry.Value.Status;
                if (activity != null)
                {
                    var tags = new ActivityTagsCollection(entry.Value.Data);
                    tags.Add("health_check.name", entry.Key);
                    tags.Add("health_check.description", entry.Value.Description);
                    tags.Add("health_check.tags", entry.Value.Tags);
                    tags.Add("health_check.state", entry.Value.Status);
                    activity.AddEvent(new ActivityEvent(
                            $"health_check.state {entry.Key}",
                            DateTimeOffset.Now,
                            tags));
                }
            }

            activity?.Stop();
        }
    }
}

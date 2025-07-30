// Copyright The OpenTelemetry Authors
// SPDX-License-Identifier: Apache-2.0

using OpenTelemetry.Instrumentation.HealthCheck;
using OpenTelemetry.Internal;

// ReSharper disable once CheckNamespace
namespace OpenTelemetry.Trace;

/// <summary>
/// Extension methods to simplify registering of dependency instrumentation.
/// </summary>
public static class TraceProviderBuilderExtensions
{
    /// <summary>
    /// Enables the collection of Health Check reports as Open Telemetry Events.
    /// </summary>
    /// <param name="builder"><see cref="TraceProviderBuilderExtensions"/> being configured.</param>
    /// <returns>The instance of <see cref="TraceProviderBuilderExtensions"/> to chain the calls.</returns>
    public static TracerProviderBuilder AddHealthCheckInstrumentation(
        this TracerProviderBuilder builder)
    {
        Guard.ThrowIfNull(builder);

        builder.AddSource(HealthCheckInstrumentationActivitySource.ActivitySourceName);

        return builder;
    }
}

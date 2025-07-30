// Copyright The OpenTelemetry Authors
// SPDX-License-Identifier: Apache-2.0

using System.Diagnostics;

namespace OpenTelemetry.Instrumentation.HealthCheck;

/// <summary>
/// It is recommended to use a custom type to hold references for ActivitySource.
/// This avoids possible type collisions with other components in the DI container.
/// </summary>
internal static class HealthCheckInstrumentationActivitySource
{
    internal const string ActivitySourceName = "OpenTelemetry.Instrumentation.HealthCheck";

    public static ActivitySource ActivitySource { get; } = new ActivitySource(ActivitySourceName);
}

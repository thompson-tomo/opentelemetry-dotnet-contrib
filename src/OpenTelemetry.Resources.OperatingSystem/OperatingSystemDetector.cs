// Copyright The OpenTelemetry Authors
// SPDX-License-Identifier: Apache-2.0

using static OpenTelemetry.Resources.OperatingSystem.OperatingSystemSemanticConventions;

namespace OpenTelemetry.Resources.OperatingSystem;

/// <summary>
/// Operating system detector.
/// </summary>
internal sealed class OperatingSystemDetector : IResourceDetector
{
    /// <summary>
    /// Detects the resource attributes from the operating system.
    /// </summary>
    /// <returns>Resource with key-value pairs of resource attributes.</returns>
    public Resource Detect()
    {

        var attributes = new List<KeyValuePair<string, object>>()
            {
            new(AttributeOperatingSystemBuild, Environment.OSVersion.Version.Build),
            new(AttributeOperatingSystemDescription, Environment.OSVersion.VersionString),
            new(AttributeOperatingSystemVersion, Environment.OSVersion.Version.ToString()),
            };
        var osType = GetOSType();

        if (osType != null)
        {
            attributes.Add(new KeyValuePair<string, object>(AttributeOperatingSystemType, osType));
        }
        var osName = GetOSName();

        return new Resource(attributes);
    }

    private static string? GetOSType()
    {
#if NET5_0_OR_GREATER
        if (System.OperatingSystem.IsAndroid())
        {
            return OperatingSystemsValues.Android;
        }
        if (System.OperatingSystem.IsBrowser())
        {
            return OperatingSystemsValues.Android;
        }
        if (System.OperatingSystem.IsFreeBSD())
        {
            return OperatingSystemsValues.FreeBSD;
        }
        if (System.OperatingSystem.IsIOS())
        {
            return OperatingSystemsValues.IOS;
        }
        if (System.OperatingSystem.IsLinux())
        {
            return OperatingSystemsValues.Linux;
        }
        if (System.OperatingSystem.IsMacCatalyst())
        {
            return OperatingSystemsValues.MacCatalyst;
        }
        if (System.OperatingSystem.IsMacOS())
        {
            return OperatingSystemsValues.MacOS;
        }
        if (System.OperatingSystem.IsTvOS())
        {
            return OperatingSystemsValues.TvOS;
        }
        if (System.OperatingSystem.IsWatchOS())
        {
            return OperatingSystemsValues.WatchOS;
        }
        if (System.OperatingSystem.IsWindows())
        {
            return OperatingSystemsValues.Windows;
        }
#else
        var platform = Environment.OSVersion.Platform;
        if (platform == PlatformID.Win32NT)
        {
            return OperatingSystemsValues.Windows;
        }

        if (platform == PlatformID.MacOSX)
        {
            return OperatingSystemsValues.MacOS;
        }

        if (platform == PlatformID.Unix)
        {
            return OperatingSystemsValues.Linux;
        }
#endif
        return null;
    }
}

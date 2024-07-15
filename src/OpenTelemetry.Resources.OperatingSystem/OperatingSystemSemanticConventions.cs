// Copyright The OpenTelemetry Authors
// SPDX-License-Identifier: Apache-2.0

namespace OpenTelemetry.Resources.OperatingSystem;

internal static class OperatingSystemSemanticConventions
{
    public const string AttributeOperatingSystemType = "os.type";
    public const string AttributeOperatingSystemName= "os.name";
    public const string AttributeOperatingSystemVersion = "os.version";
    public const string AttributeOperatingSystemDescription = "os.description";
    public const string AttributeOperatingSystemBuild = "os.build_id";

    public static class OperatingSystemsValues
    {
        public const string Windows = "windows";

        public const string Linux = "linux";

        public const string Darwin = "darwin";

        public const string FreeBSD = "freebsd";

        public const string NetBSD = "netbsd";

        public const string OpenBSD = "openbsd";

        public const string DragonFlyBSD = "dragonflybsd";

        public const string HPUX = "hpux";

        public const string AIX = "aix";

        public const string Solaris = "solaris";

        public const string ZOS = "z_os";

        public const string Android = "android";

        public const string IOS = "ios";

        public const string MacCatalyst = "maccatalyst";

        public const string MacOS = "macosx";

        public const string TvOS = "tvos";

        public const string WatchOS = "watchos";
    }
}

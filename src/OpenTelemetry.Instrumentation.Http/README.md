# HttpClient and HttpWebRequest instrumentation for OpenTelemetry

| Status      |           |
| ----------- | --------- |
| Stability   | [Stable](../../README.md#stable) |
| Code Owners | [@open-telemetry/dotnet-contrib-maintainers](https://github.com/orgs/open-telemetry/teams/dotnet-contrib-maintainers) |

[![NuGet](https://img.shields.io/nuget/v/OpenTelemetry.Instrumentation.Http.svg)](https://www.nuget.org/packages/OpenTelemetry.Instrumentation.Http)
[![NuGet](https://img.shields.io/nuget/dt/OpenTelemetry.Instrumentation.Http.svg)](https://www.nuget.org/packages/OpenTelemetry.Instrumentation.Http)
[![codecov.io](https://codecov.io/gh/open-telemetry/opentelemetry-dotnet-contrib/branch/main/graphs/badge.svg?flag=unittests-Instrumentation.Http)](https://app.codecov.io/gh/open-telemetry/opentelemetry-dotnet-contrib?flags[0]=unittests-Instrumentation.Http)

This is an [Instrumentation
Library](https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/glossary.md#instrumentation-library),
which instruments
[System.Net.Http.HttpClient](https://docs.microsoft.com/dotnet/api/system.net.http.httpclient)
and
[System.Net.HttpWebRequest](https://docs.microsoft.com/dotnet/api/system.net.httpwebrequest)
and collects metrics and traces about outgoing HTTP requests.

This component is based on the
[v1.23](https://github.com/open-telemetry/semantic-conventions/tree/v1.23.0/docs/http)
of http semantic conventions. For details on the default set of attributes that
are added, checkout [Traces](#traces) and [Metrics](#metrics) sections below.

## Steps to enable OpenTelemetry.Instrumentation.Http

### Step 1: Install Package

Add a reference to the
[`OpenTelemetry.Instrumentation.Http`](https://www.nuget.org/packages/OpenTelemetry.Instrumentation.Http)
package. Also, add any other instrumentations & exporters you will need.

```shell
dotnet add package OpenTelemetry.Instrumentation.Http
```

### Step 2: Enable HTTP Instrumentation at application startup

HTTP instrumentation must be enabled at application startup.

#### Traces

Starting with .NET 9, trace instrumentation is natively implemented, and the
HttpClient library emits attributes defined in the
[OpenTelemetry Specification](https://github.com/open-telemetry/semantic-conventions/blob/v1.28.0/docs/http/http-spans.md).
When running on .NET 9+ this instrumentation library will not add/change/override
any attributes set by the native instrumentation but it is still required for
performing context propagation using the OpenTelemetry SDK and supports additional
features not available in runtime (enrichment, filtering, etc.).

The following example demonstrates adding `HttpClient` instrumentation with the
extension method `.AddHttpClientInstrumentation()` on `TracerProviderBuilder` to
a console application. This example also sets up the OpenTelemetry Console
Exporter, which requires adding the package
[`OpenTelemetry.Exporter.Console`](https://github.com/open-telemetry/opentelemetry-dotnet/blob/main/src/OpenTelemetry.Exporter.Console/README.md)
to the application.

```csharp
using OpenTelemetry;
using OpenTelemetry.Trace;

public class Program
{
    public static void Main(string[] args)
    {
        using var tracerProvider = Sdk.CreateTracerProviderBuilder()
            .AddHttpClientInstrumentation()
            .AddConsoleExporter()
            .Build();
    }
}
```

Following list of attributes are added by default on activity. See
[http-spans](https://github.com/open-telemetry/semantic-conventions/tree/v1.23.0/docs/http/http-spans.md)
for more details about each individual attribute:

* `error.type`
* `http.request.method`
* `http.request.method_original`
* `http.response.status_code`
* `network.protocol.version`
* `server.address`
* `server.port`
* `url.full` - By default, the values in the query component of the url are
  replaced with the text `Redacted`. For example, `?key1=value1&key2=value2`
  becomes `?key1=Redacted&key2=Redacted`. You can disable this redaction by
  setting the environment variable
  `OTEL_DOTNET_EXPERIMENTAL_HTTPCLIENT_DISABLE_URL_QUERY_REDACTION` to `true`.

[Enrich Api](#enrich-httpclient-api) can be used if any additional attributes are
required on activity.

#### Metrics

The following example demonstrates adding `HttpClient` instrumentation with the
extension method `.AddHttpClientInstrumentation()` on `MeterProviderBuilder` to
a console application. This example also sets up the OpenTelemetry Console
Exporter, which requires adding the package
[`OpenTelemetry.Exporter.Console`](https://github.com/open-telemetry/opentelemetry-dotnet/blob/main/src/OpenTelemetry.Exporter.Console/README.md)
to the application.

```csharp
using OpenTelemetry;
using OpenTelemetry.Metrics;

public class Program
{
    public static void Main(string[] args)
    {
        using var meterProvider = Sdk.CreateMeterProviderBuilder()
            .AddHttpClientInstrumentation()
            .AddConsoleExporter()
            .Build();
    }
}
```

Refer to this [example](https://github.com/open-telemetry/opentelemetry-dotnet/blob/main/examples/AspNetCore/Program.cs)
to see how to enable this instrumentation in an ASP.NET Core application.

Refer to this
[example](https://github.com/open-telemetry/opentelemetry-dotnet-contrib/blob/main/src/OpenTelemetry.Instrumentation.AspNet/README.md)
to see how to enable this instrumentation in an ASP.NET application.

Following list of attributes are added by default on
`http.client.request.duration` metric. See
[http-metrics](https://github.com/open-telemetry/semantic-conventions/tree/v1.23.0/docs/http/http-metrics.md)
for more details about each individual attribute. `.NET8.0` and above supports
additional metrics, see [list of metrics produced](#list-of-metrics-produced) for
more details.

* `error.type`
* `http.request.method`
* `http.response.status_code`
* `network.protocol.version`
* `server.address`
* `server.port`
* `url.scheme`

#### List of metrics produced

When the application targets `NETFRAMEWORK`, `.NET6.0` or `.NET7.0`, the
instrumentation emits the following metric:

| Name                              | Details                                                                                                                                                 |
|-----------------------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------|
| `http.client.request.duration`    | [Specification](https://github.com/open-telemetry/semantic-conventions/blob/release/v1.23.x/docs/http/http-metrics.md#metric-httpclientrequestduration) |

Starting from `.NET8.0`, metrics instrumentation is natively implemented, and
the HttpClient library has incorporated support for [built-in
metrics](https://learn.microsoft.com/dotnet/core/diagnostics/built-in-metrics-system-net)
following the OpenTelemetry semantic conventions. The library includes additional
metrics beyond those defined in the
[specification](https://github.com/open-telemetry/semantic-conventions/blob/v1.23.0/docs/http/http-metrics.md),
covering additional scenarios for HttpClient users. When the application targets
`.NET8.0` and newer versions, the instrumentation library automatically enables
all `built-in` metrics by default.

Note that the `AddHttpClientInstrumentation()` extension simplifies the process
of enabling all built-in metrics via a single line of code. Alternatively, for
more granular control over emitted metrics, you can utilize the `AddMeter()`
extension on `MeterProviderBuilder` for meters listed in
[built-in-metrics-system-net](https://learn.microsoft.com/dotnet/core/diagnostics/built-in-metrics-system-net).
Using `AddMeter()` for metrics activation eliminates the need to take dependency
on the instrumentation library package and calling
`AddHttpClientInstrumentation()`.

If you utilize `AddHttpClientInstrumentation()` and wish to exclude unnecessary
metrics, you can utilize
[Views](https://github.com/open-telemetry/opentelemetry-dotnet/tree/main/docs/metrics/customizing-the-sdk#drop-an-instrument)
to achieve this.

> [!NOTE]
> There is no difference in features or emitted metrics when enabling metrics
using `AddMeter()` or `AddHttpClientInstrumentation()` on `.NET8.0` and newer
versions.
<!-- This comment is to make sure the two notes above and below are not merged -->
> [!NOTE]
> The `http.client.request.duration` metric is emitted in `seconds` as per the
semantic convention. While the convention [recommends using custom histogram
buckets](https://github.com/open-telemetry/semantic-conventions/blob/release/v1.23.x/docs/http/http-metrics.md)
, this feature is not yet available via .NET Metrics API. A
[workaround](https://github.com/open-telemetry/opentelemetry-dotnet/pull/4820)
has been included in OTel SDK starting version `1.6.0` which applies recommended
buckets by default for `http.client.request.duration`. This applies to all
targeted frameworks.

## Advanced configuration

### Tracing

This instrumentation can be configured to change the default behavior by using
`HttpClientTraceInstrumentationOptions`. It is important to note that there are
differences between .NET Framework and newer .NET/.NET Core runtimes which
govern what options are used. On .NET Framework, `HttpClient` uses the
`HttpWebRequest` API. On .NET & .NET Core, `HttpWebRequest` uses the
`HttpClient` API. As such, depending on the runtime, only one half of the
"filter" & "enrich" options are used.

#### .NET & .NET Core

##### Filter HttpClient API

This instrumentation by default collects all the outgoing HTTP requests. It
allows filtering of requests by using the `FilterHttpRequestMessage` function
option. This defines the condition for allowable requests. The filter function
receives the request object (`HttpRequestMessage`) representing the outgoing
request and does not collect telemetry about the request if the filter function
returns `false` or throws an exception.

The following code snippet shows how to use `FilterHttpRequestMessage` to only
allow GET requests.

```csharp
using var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .AddHttpClientInstrumentation(
        // Note: Only called on .NET & .NET Core runtimes.
        (options) => options.FilterHttpRequestMessage =
            (httpRequestMessage) =>
            {
                // Example: Only collect telemetry about HTTP GET requests.
                return httpRequestMessage.Method.Equals(HttpMethod.Get);
            })
    .AddConsoleExporter()
    .Build();
```

It is important to note that this `FilterHttpRequestMessage` option is specific
to this instrumentation. OpenTelemetry has a concept of a
[Sampler](https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/trace/sdk.md#sampling),
and the `FilterHttpRequestMessage` option does the filtering *after* the Sampler
is invoked.

##### Enrich HttpClient API

This instrumentation library provides options that can be used to
enrich the activity with additional information. These actions are called
only when `activity.IsAllDataRequested` is `true`. It contains the activity
itself (which can be enriched) and the actual raw object.

`HttpClientTraceInstrumentationOptions` provides 3 enrich options:
`EnrichWithHttpRequestMessage`, `EnrichWithHttpResponseMessage` and
`EnrichWithException`. These are based on the raw object that is passed in to
the action to enrich the activity.

Example:

```csharp
using System.Net.Http;

var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .AddHttpClientInstrumentation((options) =>
    {
        // Note: Only called on .NET & .NET Core runtimes.
        options.EnrichWithHttpRequestMessage = (activity, httpRequestMessage) =>
        {
            activity.SetTag("requestVersion", httpRequestMessage.Version);
        };
        // Note: Only called on .NET & .NET Core runtimes.
        options.EnrichWithHttpResponseMessage = (activity, httpResponseMessage) =>
        {
            activity.SetTag("responseVersion", httpResponseMessage.Version);
        };
        // Note: Called for all runtimes.
        options.EnrichWithException = (activity, exception) =>
        {
            activity.SetTag("stackTrace", exception.StackTrace);
        };
    })
    .Build();
```

#### .NET Framework

##### Filter HttpWebRequest API

This instrumentation by default collects all the outgoing HTTP requests. It
allows filtering of requests by using the `FilterHttpWebRequest` function
option. This defines the condition for allowable requests. The filter function
receives the request object (`HttpWebRequest`) representing the outgoing request
and does not collect telemetry about the request if the filter function returns
`false` or throws an exception.

The following code snippet shows how to use `FilterHttpWebRequest` to only allow
GET requests.

```csharp
using var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .AddHttpClientInstrumentation(
        // Note: Only called on .NET Framework.
        (options) => options.FilterHttpWebRequest =
            (httpWebRequest) =>
            {
                // Example: Only collect telemetry about HTTP GET requests.
                return httpWebRequest.Method.Equals(HttpMethod.Get.Method);
            })
    .AddConsoleExporter()
    .Build();
```

It is important to note that this `FilterHttpWebRequest` option is specific to
this instrumentation. OpenTelemetry has a concept of a
[Sampler](https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/trace/sdk.md#sampling),
and the `FilterHttpWebRequest` option does the filtering *after* the Sampler is
invoked.

##### Enrich HttpWebRequest API

This instrumentation library provides options that can be used to
enrich the activity with additional information. These actions are called
only when `activity.IsAllDataRequested` is `true`. It contains the activity
itself (which can be enriched) and the actual raw object.

`HttpClientTraceInstrumentationOptions` provides 3 enrich options:
`EnrichWithHttpWebRequest`, `EnrichWithHttpWebResponse` and
`EnrichWithException`. These are based on the raw object that is passed in to
the action to enrich the activity.

Example:

```csharp
using System.Net;

var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .AddHttpClientInstrumentation((options) =>
    {
        // Note: Only called on .NET Framework.
        options.EnrichWithHttpWebRequest = (activity, httpWebRequest) =>
        {
            activity.SetTag("requestVersion", httpWebRequest.Version);
        };
        // Note: Only called on .NET Framework.
        options.EnrichWithHttpWebResponse = (activity, httpWebResponse) =>
        {
            activity.SetTag("responseVersion", httpWebResponse.Version);
        };
        // Note: Called for all runtimes.
        options.EnrichWithException = (activity, exception) =>
        {
            activity.SetTag("stackTrace", exception.StackTrace);
        };
    })
    .Build();
```

[Processor](https://github.com/open-telemetry/opentelemetry-dotnet/blob/main/docs/trace/extending-the-sdk/README.md#processor),
is the general extensibility point to add additional properties to any activity.
The `Enrich` option is specific to this instrumentation, and is provided to get
access to raw request, response, and exception objects.

When overriding the default settings provided by instrumentation or adding
additional telemetry, it is important to consider the sequence of callbacks.

It is generally recommended to use `EnrichWithHttpResponseMessage` or
`EnrichWithHttpWebResponse` for any activity enrichment that does not require
access to exceptions or request object in case of .NET Framework, as the
instrumentation library populates all telemetry following the [OTel
specification](https://github.com/open-telemetry/semantic-conventions/tree/v1.27.0/docs/http)
before this callback. The following is the sequence in which these callbacks are
executed:

1) Processor `OnStart`
2) `EnrichWithHttpRequestMessage` (.NET) / `EnrichWithHttpWebRequest` (.NET Framework)
3) `EnrichWithException` both
4) `EnrichWithHttpResponseMessage` (.NET) / `EnrichWithHttpWebResponse` (.NET Framework)
5) Processor `OnEnd`

As an example, if you need to override the default DisplayName set by the
library you can do so as follows:

```csharp
.AddHttpClientInstrumentation(o =>
{
  o.EnrichWithHttpResponseMessage = (activity, response) =>
  {
      // .NET only, access request object if needed.
      // response.RequestMessage
      activity.DisplayName = "CustomDisplayName";

      // Overrides the value
      activity.SetTag("url.full", "CustomUrl");

      // Removes the tag
      activity.SetTag("network.protocol.version", null);
  };
});
```

#### RecordException

This instrumentation automatically sets Activity Status to Error if the Http
StatusCode is >= 400. Additionally, `RecordException` feature may be turned on,
to store the exception to the Activity itself as ActivityEvent.

## Activity duration and http.client.request.duration metric calculation

`Activity.Duration` and `http.client.request.duration` values represents the
time the underlying client handler takes to complete the request. Completing the
request includes the time up to reading response headers from the network
stream. It doesn't include the time spent reading the response body.

## Troubleshooting

This component uses an
[EventSource](https://docs.microsoft.com/dotnet/api/system.diagnostics.tracing.eventsource)
with the name "OpenTelemetry-Instrumentation-Http" for its internal logging.
Please refer to [SDK
troubleshooting](https://github.com/open-telemetry/opentelemetry-dotnet/blob/main/src/OpenTelemetry/README.md#troubleshooting)
for instructions on seeing these internal logs.

## References

* [OpenTelemetry Project](https://opentelemetry.io/)

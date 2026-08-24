# Nestgrid.Response.Http.Client

`Nestgrid.Response.Http.Client` interprets an HTTP response as a Nestgrid `Result` or `Result<T>`. It is the client-side counterpart to the server adapters, but it does not reverse server-side result mappings.

## Installation

```bash
dotnet add package Nestgrid.Response.Http.Client
```

## Quick start

Use the reader when the response is already available:

```csharp
using Nestgrid.Response.Http.Client;

var reader = new NestgridResponseReader(
    new NestgridResponseClientOptions(NestgridResponsePayloadMode.ValueOnly));

using HttpResponseMessage response = await httpClient.GetAsync("/licences/17");
Result<Licence> result = await reader.ReadAsync<Licence>(response);
```

Use the thin convenience method when the package should send and interpret a request. The response created by `HttpClient` is disposed by the convenience method; the caller-created request and `HttpClient` remain caller-owned:

```csharp
using var request = new HttpRequestMessage(HttpMethod.Get, "/licences/17");
Result<Licence> result = await httpClient.SendAndReadNestgridResponseAsync<Licence>(
    request,
    reader,
    cancellationToken);
```

Authentication, other handlers, retries, resilience, logging, telemetry and `IHttpClientFactory` registration remain normal consumer-owned `HttpClient` composition.

## Payload modes

The mode is explicit and is never inferred from JSON shape.

| Mode | Generic success | Non-generic success | Failure |
| --- | --- | --- | --- |
| `FullResult` | Envelope containing `Value` and `Messages` | Envelope containing `Messages` | Envelope containing `Messages` |
| `ValueOnly` | Body is the declared `T` value | Envelope containing `Messages`, or an empty 200/201/202 body | Envelope containing `Messages` |

`204 No Content` always returns the existing typed or non-generic `NoContent` result. A generic operation with an empty 200, 201 or 202 body is a protocol failure because its declared value is missing.

## HTTP outcome mappings

The client interprets the observed HTTP status. It does not reconstruct the server-side `ResultStatus` that may have produced it.

| HTTP status | Client result status |
| --- | --- |
| 200 | `Ok` |
| 201 | `Created` |
| 202 | `Accepted` |
| 204 | `NoContent` |
| 400 | `Invalid` |
| 401 | `Unauthorized` |
| 403 | `Forbidden` |
| 404 | `NotFound` |
| 409 | `Conflict` |
| 422 | `Failed` |
| 500–599 | `Error` |

Mappings can be extended or replaced through the immutable client policy. Three-hundred-series and otherwise unmapped statuses are protocol failures by default.

## JSON and media types

The package supports JSON only through the centrally managed `System.Text.Json` baseline. `application/json` and `+json` media types are accepted. A missing media type is accepted for compatibility with legacy HTTP responses. Plain text, XML and other non-JSON media types are rejected with `NestgridResponseProtocolException`.

The client options snapshot the supported serializer settings at construction, including naming, casing, comments, trailing commas, null handling, encoder, depth, indentation and custom converters. A reader takes another snapshot when it is constructed, so later mutation of the caller's options cannot change that reader's behaviour.

## Messages and failures

Wire messages preserve message text, code, property and severity through the existing `ResultMessages` factories. Malformed JSON, invalid envelopes, wrong payload representations, invalid messages and unsupported statuses raise `NestgridResponseProtocolException` without exposing raw response bodies or sensitive headers.

Network, DNS, TLS, timeout and cancellation failures remain standard `HttpClient` exceptions. They are not converted into application results.

## Scope

This package does not provide authentication, endpoint-specific clients, generic HTTP abstractions, global handlers, retries, resilience, DI registration, logging or telemetry. Consumers own those concerns and the lifetime of `HttpClient`.

# Nestgrid.Response.Http.Client

`Nestgrid.Response.Http.Client` interprets explicit Nestgrid HTTP responses as `Result` and `Result<T>` values.

The package supports `FullResult` and `ValueOnly` payload modes, client-owned HTTP status mappings and normal `HttpClient` composition. It does not provide authentication, retries, resilience, dependency injection registration or endpoint-specific clients. Consumers own `HttpClient` lifetime and handler configuration.

The client interprets observed HTTP outcomes; it does not reverse the server-side `Nestgrid.Response.Http` mapping. Malformed, mismatched and unmapped responses raise `NestgridResponseProtocolException`. Network and cancellation exceptions remain standard `HttpClient` exceptions.

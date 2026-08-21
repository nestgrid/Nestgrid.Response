using System.Net;
using System.Net.Http;
using Nestgrid.Response.Http.Client;

using var licenceClient = new HttpClient(new StaticHandler(
    new HttpResponseMessage(HttpStatusCode.OK)
    {
        Content = new StringContent("{\"Value\":{\"Id\":17,\"Name\":\"Standard\"},\"Messages\":[]}")
    }));
using var licenceRequest = new HttpRequestMessage(HttpMethod.Get, "https://licence-service.example/licences/17");
var licence = await licenceClient.SendNestgridResponseAsync<Licence>(
    licenceRequest,
    new NestgridResponseReader());

using var financeClient = new HttpClient(new StaticHandler(
    new HttpResponseMessage(HttpStatusCode.OK)
    {
        Content = new StringContent("{\"InvoiceId\":42,\"Currency\":\"GBP\"}")
    }));
using var financeRequest = new HttpRequestMessage(HttpMethod.Get, "https://finance.example/invoices/42");
var finance = await financeClient.SendNestgridResponseAsync<Invoice>(
    financeRequest,
    new NestgridResponseReader(new NestgridResponseClientOptions(NestgridResponsePayloadMode.ValueOnly)));

Console.WriteLine($"Licence: {licence.Value?.Name} ({licence.Status})");
Console.WriteLine($"Invoice: {finance.Value?.InvoiceId} {finance.Value?.Currency} ({finance.Status})");

internal sealed record Licence(int Id, string Name);

internal sealed record Invoice(int InvoiceId, string Currency);

internal sealed class StaticHandler : HttpMessageHandler
{
    private readonly HttpResponseMessage response;

    public StaticHandler(HttpResponseMessage response) => this.response = response;

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken) => Task.FromResult(response);
}

using System.Net;
using Nestgrid.Response.Http.Client;
using Nestgrid.Response.Http.Client.Sample;

using var licenceClient = new HttpClient(new StaticHandler(
    new HttpResponseMessage(HttpStatusCode.OK)
    {
        Content = new StringContent("{\"Value\":{\"Id\":17,\"Name\":\"Standard\"},\"Messages\":[]}", System.Text.Encoding.UTF8, "application/json")
    }));

using var licenceRequest = new HttpRequestMessage(HttpMethod.Get, "https://licence-service.example/licences/17");
var licence = await licenceClient.SendAndReadNestgridResponseAsync<Licence>(
    licenceRequest,
    new NestgridResponseReader());

using var financeClient = new HttpClient(new StaticHandler(
    new HttpResponseMessage(HttpStatusCode.OK)
    {
        Content = new StringContent("{\"InvoiceId\":42,\"Currency\":\"GBP\"}", System.Text.Encoding.UTF8, "application/json")
    }));

using var financeRequest = new HttpRequestMessage(HttpMethod.Get, "https://finance.example/invoices/42");
var finance = await financeClient.SendAndReadNestgridResponseAsync<Invoice>(
    financeRequest,
    new NestgridResponseReader(new NestgridResponseClientOptions(NestgridResponsePayloadMode.ValueOnly)));

Console.WriteLine($"Licence: {licence.Value?.Name} ({licence.Status})");
Console.WriteLine($"Invoice: {finance.Value?.InvoiceId} {finance.Value?.Currency} ({finance.Status})");

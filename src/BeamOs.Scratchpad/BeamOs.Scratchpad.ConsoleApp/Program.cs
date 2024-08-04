// See https://aka.ms/new-console-template for more information
using BeamOs.ApiClient.Builders;
using BeamOs.Contracts.PhysicalModel.Common;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOs.Scratchpad.ConsoleApp;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("Hello, World!");

var serviceProvider = new ServiceCollection().AddHttpClient().BuildServiceProvider();

var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();

var client = httpClientFactory.CreateClient();
client.BaseAddress = new("https://localhost:7111");

CustomModelBuilder customModelBuilder = new();
await customModelBuilder.InitializeAsync();

string connectionId = customModelBuilder.ScratchpadId;
ModelResponse modelResponse = customModelBuilder.ToResponseWithLocalIds();

var json_ = System.Text.Json.JsonSerializer.Serialize<BeamOsEntityContractBase>(modelResponse);

using var request_ = new System.Net.Http.HttpRequestMessage();
var content_ = new System.Net.Http.StringContent(json_);
content_.Headers.ContentType = System
    .Net
    .Http
    .Headers
    .MediaTypeHeaderValue
    .Parse("application/json");

request_.Content = content_;
request_.Method = new System.Net.Http.HttpMethod("POST");
request_
    .Headers
    .Accept
    .Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

var urlBuilder_ = new System.Text.StringBuilder();

urlBuilder_.Append($"scratchpad-entity?connectionId={connectionId}");

var url_ = urlBuilder_.ToString();
request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

try
{
    var response_ = await client
        .SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead)
        .ConfigureAwait(false);
}
catch (Exception) { }
while (true)
{
    await Task.Delay(100000);
}

using BeamOs.Ai;
using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<AiApiPlugin>();
builder.Services.AddSingleton<UriProvider>();
builder
    .Services
    .AddHttpClient<IStructuralAnalysisApiClientV1, StructuralAnalysisApiClientV1>(
        client => client.BaseAddress = new("http://localhost:5223")
    );

// builder.Services.AddLogging(b => b.AddConsole().SetMinimumLevel(LogLevel.Trace));
builder.Services.AddSingleton<MessageHandler1>();

#pragma warning disable SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
builder
    .Services
    .AddKernel()
    .AddOllamaChatCompletion("llama3.2:3b", new Uri("http://localhost:11434"));
#pragma warning restore SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

builder
    .Services
    .AddHttpClient("llamaClient", config => config.BaseAddress = new("http://localhost:11434"))
    .AddHttpMessageHandler<MessageHandler1>()
    .SetHandlerLifetime(Timeout.InfiniteTimeSpan)
    .ConfigureHttpClient(client => client.Timeout = TimeSpan.FromMinutes(10));

var app = builder.Build();
var x = app.Configuration;

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapPost(
    "/chat",
    (
        [FromBody] ChatRequest command,
        AiApiPlugin aiApiPlugin,
        UriProvider uriProvider,
        IHttpClientFactory httpClientFactory,
        CancellationToken ct = default
    ) =>
    {
        var httpClient = httpClientFactory.CreateClient("llamaClient");
        var chatCommandHandler = new ChatCommandHandler(aiApiPlugin, uriProvider, httpClient);
        return chatCommandHandler.ExecuteChatAsync(command, ct);
    }
);

app.UseHttpsRedirection();

app.Run();

using System.Text;
using System.Text.Json;
using BeamOs.CodeGen.AiApiClient;
using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Contracts;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BeamOs.WebApp.Components.Features.AiAssistant;

public partial class AiAssistant(
    IHttpClientFactory httpClientFactory,
    IAiApiClient aiApiClient,
    ISnackbar snackbar
)
{
    private readonly HttpClient httpClient = httpClientFactory.CreateClient("default");

    [Parameter]
    public Guid ModelId { get; init; }

    private bool expanded = false;
    private bool apiKeySubmitted = false;
    private string apiKey = "";
    private string userMessage = "";
    private bool isLoading = false;
    private ElementReference chatContainer;

    private List<ChatMessage> chatMessages = new();

    private string chatContainerClass =>
        $"flex-1 overflow-y-auto p-4 space-y-4 {(this.shouldScroll ? "force-scroll" : "")}";

    private bool shouldScroll;

    private class ChatMessage
    {
        public string Text { get; set; } = "";
        public bool IsUser { get; set; }
    }

    private void ToggleChat()
    {
        this.expanded = !this.expanded;
    }

    private async Task SendMessage()
    {
        if (string.IsNullOrWhiteSpace(this.userMessage))
            return;

        // Add user message to chat

        this.chatMessages.Add(new ChatMessage { Text = this.userMessage, IsUser = true });
        this.isLoading = true;
        await this.ScrollToBottom();

        await this.GetAiResponse();

        this.userMessage = "";
        this.isLoading = false;
        await this.ScrollToBottom();
    }

    private async Task GetAiResponse()
    {
        // var aiUri = new UriBuilder(uriProvider.AiUri) { Path = "api/github-models-chat" };
        var requestBody = new GithubModelsChatRequest()
        {
            ApiKey = this.apiKey,
            Message = this.userMessage,
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await aiApiClient.GithubModelsChatAsync(this.ModelId, requestBody);

        if (response.IsError)
        {
            snackbar.Add($"Error: {response.Error.Description}", Severity.Error);
            return;
        }

        ChatMessage chatMessage = new() { IsUser = false, Text = response.Value.Message };
        this.chatMessages.Add(chatMessage);

        this.StateHasChanged();
        await this.ScrollToBottom();
    }

    private async Task ScrollToBottom()
    {
        this.shouldScroll = true;
        await Task.Delay(1); // Force a render
        this.shouldScroll = false;
        await Task.Delay(1); // Reset
    }
}

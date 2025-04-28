using System.ClientModel;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;

namespace BeamOs.Ai;

public class OpenAiChatClientFactory
{
    public IChatClient CreateChatClient(IConfiguration configuration)
    {
        var credential = new ApiKeyCredential(
            configuration["OpenAI:Key"]
                ?? throw new InvalidOperationException(
                    "Missing configuration: GitHubModels:Token. See the README for details."
                )
        );
        var openAIOptions = new OpenAIClientOptions()
        {
            Endpoint = new Uri("https://models.inference.ai.azure.com"),
        };

        var ghModelsClient = new OpenAIClient(credential, openAIOptions);
        var chatClient = ghModelsClient.AsChatClient("gpt-4o-mini");
        return chatClient;
    }

    public IChatClient CreateGithubModelsChatClient(string apiKey)
    {
        var credential = new ApiKeyCredential(
            apiKey
                ?? throw new InvalidOperationException(
                    "Missing configuration: GitHubModels:Token. See the README for details."
                )
        );
        var openAIOptions = new OpenAIClientOptions()
        {
            Endpoint = new Uri("https://models.inference.ai.azure.com"),
        };

        var ghModelsClient = new OpenAIClient(credential, openAIOptions);
        var chatClient = ghModelsClient.AsChatClient("gpt-4o-mini");
        return chatClient;
    }
}

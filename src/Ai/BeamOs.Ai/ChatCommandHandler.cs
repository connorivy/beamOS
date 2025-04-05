using System.Text;
using System.Threading.Tasks;
using BeamOs.Common.Api;
using BeamOs.Common.Application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;
using Microsoft.SemanticKernel.Plugins.OpenApi;

namespace BeamOs.Ai;

public class ChatCommandHandler(
    AiApiPlugin aiApiPlugin,
    UriProvider uriProvider,
    HttpClient httpClient
) : IAsyncEnumerableCommandHandler<ChatRequest, string>
{
    private Kernel? kernel;
    private IChatCompletionService? chatCompletionService;

    public async IAsyncEnumerable<string> ExecuteAsync(
        ChatRequest command,
        CancellationToken ct = default
    )
    {
        // Lazy initialize kernel when first needed
        this.kernel ??= await this.BuildKernel();

        var agent = new ChatCompletionAgent()
        {
            Instructions = """
                Your job is to make changes to structural analysis models based off of user requests. 
                You will be provided a modelId. DO NOT MODIFY MODELS WITH IDS OTHER THAN THE ONE YOU RECEIVE.

                You must ALWAYS either invoke one or more functions in the StructuralAnalysisApi or tell the user
                that you cannot help them. You must NEVER just answer the user without invoking a function.
                """,
            Kernel = this.kernel,
            Arguments = new KernelArguments(
                new PromptExecutionSettings
                {
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Required()
                }
            ),
        };

        ChatHistory chatHistory =
        [
            new ChatMessageContent(AuthorRole.User, 
                           command.Message)
        ];

        StringBuilder sb = new();
        await foreach (var response in agent.InvokeAsync(chatHistory, cancellationToken: ct))
        {
            sb.Append(response.Content);
        }

        yield return sb.ToString();
    }

    public async IAsyncEnumerable<string> ExecuteChatAsync(
        ChatRequest command,
        CancellationToken ct = default
    )
    {
        // Lazy initialize kernel when first needed
        this.kernel ??= await this.BuildKernel();

        ChatHistory chatHistory =
        [
            new ChatMessageContent(AuthorRole.System, """
                Users are going to ask you to perform tasks related to their structural analysis models. 
                Your job is to use the provided modelId to make changes to a specific model. You make these changes
                by invoking functions in the StructuralAnalysisApi.
                
                If there is an issue invoking the StructuralAnalysisApi, then please let the user know which endpoint you
                tried, and what the http response code was.
            """),
            new ChatMessageContent(AuthorRole.User,
                           command.Message)
        ];

#pragma warning disable SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        OllamaPromptExecutionSettings settings =
            new() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(), };
#pragma warning restore SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

        StringBuilder sb = new();
        var assitant = await chatCompletionService.GetChatMessageContentAsync(
            chatHistory,
            settings,
            kernel,
            cancellationToken: ct
        );
        yield return assitant.Content;
    }

    private async Task<Kernel> BuildKernel()
    {
        var builder = Kernel.CreateBuilder();

        // builder.Services.AddLogging(b => b.AddConsole().SetMinimumLevel(LogLevel.Trace));
#pragma warning disable SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        builder.AddOllamaChatCompletion("qwen2.5:7b", httpClient: httpClient);
        // builder.AddOpenAIChatCompletion("o1-mini", configuration["OpenAI:Key"]);
#pragma warning restore SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        // Add the chat completion service
        // builder.Services.AddSingleton(chatCompletionService);
        // Add AiApiPlugin with all its functions
        builder.Plugins.AddFromObject(aiApiPlugin, "StructuralAnalysisApi");

        var kernel = builder.Build();
        this.chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
        // #pragma warning disable SKEXP0040 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        //         await kernel.ImportPluginFromOpenApiAsync(
        //             "StructuralAnalysisApi",
        //             new Uri("http://localhost:5223/openapi/ai.json"),
        //             new OpenApiFunctionExecutionParameters()
        //             {
        //                 // EnableDynamicPayload = false,
        //                 EnablePayloadNamespacing = true,
        //                 OperationsToExclude =  ["DeleteModel"]
        //             },
        //             cancellationToken: default
        //         );
        // #pragma warning restore SKEXP0040 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        return kernel;
    }
}

public class MessageHandler1 : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        Console.WriteLine("Process request");
        // Call the inner handler.
        var response = await base.SendAsync(request, cancellationToken);
        Console.WriteLine("Process response");
        return response;
    }
}

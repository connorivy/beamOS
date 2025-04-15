using System.ClientModel;
using System.Runtime.CompilerServices;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using OpenAI;

namespace BeamOs.Ai;

public class GithubModelsChatCommandHandler(AiApiPlugin aiApiPlugin)
    : IAsyncEnumerableCommandHandler<GithubModelsChatRequest, string>
{
    public async IAsyncEnumerable<string> ExecuteAsync(
        GithubModelsChatRequest command,
        [EnumeratorCancellation] CancellationToken ct = default
    )
    {
        var kernel = this.BuildOpenAiKernel(command.ApiKey);

        var agent = new ChatCompletionAgent()
        {
            Instructions =
                $@"
                Your job is to make changes to structural analysis models based off of user requests. 
                The Id of the model that you can modify is {command.ModelId}. DO NOT MODIFY MODELS WITH OTHER IDS.

                You must ALWAYS either invoke one or more functions in the StructuralAnalysisApi or tell the user
                that you cannot help them. You must NEVER just answer the user without invoking a function.
                ",
            Kernel = kernel,
            Arguments = new KernelArguments(
                new PromptExecutionSettings
                {
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
                }
            ),
        };

        await foreach (
            var message in agent.InvokeAsync(
                new ChatMessageContent(AuthorRole.User, command.Message),
                cancellationToken: ct
            )
        )
        {
            yield return message.Message.Content;
        }
    }

    private Kernel BuildOpenAiKernel(string apiKey)
    {
        var builder = Kernel.CreateBuilder();
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

        builder.AddOpenAIChatCompletion("gpt-4o-mini", ghModelsClient);
        builder.Plugins.AddFromObject(aiApiPlugin, "StructuralAnalysisApi");

        var kernel = builder.Build();
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

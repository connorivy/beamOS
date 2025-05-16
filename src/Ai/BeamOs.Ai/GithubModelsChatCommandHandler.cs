using System.ClientModel;
using System.Runtime.CompilerServices;
using System.Text;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Plugins.OpenApi;
using OpenAI;

namespace BeamOs.Ai;

public class GithubModelsChatCommandHandler(
    CreateProposalCommandHandler createModelProposalCommandHandler
) : ICommandHandler<GithubModelsChatCommand, string>
{
    public async Task<Result<string>> ExecuteAsync(
        GithubModelsChatCommand command,
        CancellationToken ct = default
    )
    {
        var aiApiPlugin = new AiApiPlugin();
        var kernel = this.BuildOpenAiKernel(command.ApiKey, aiApiPlugin);

        var agent = new ChatCompletionAgent()
        {
            Instructions =
                $@"
                Your job is to make changes to and answer questions about structural analysis models based off of user requests.
                ",
            Kernel = kernel,
            Arguments = new KernelArguments(
                new PromptExecutionSettings
                {
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
                }
            ),
        };

        StringBuilder stringBuilder = new();
        await foreach (
            var message in agent.InvokeAsync(
                new ChatMessageContent(AuthorRole.User, command.Message),
                cancellationToken: ct
            )
        )
        {
            stringBuilder.Append(message.Message.Content);
        }

        var proposalResult = await createModelProposalCommandHandler.ExecuteAsync(
            new() { ModelId = command.ModelId, Body = aiApiPlugin.ModelProposalData },
            ct
        );

        if (proposalResult.IsError)
        {
            return proposalResult.Error;
        }

        return stringBuilder.ToString();
    }

    private Kernel BuildOpenAiKernel(string apiKey, AiApiPlugin aiApiPlugin)
    {
        var builder = Kernel.CreateBuilder();
        var credential = new ApiKeyCredential(apiKey);
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
        //             new Uri("http://localhost:5223/openapi/v1.json"),
        //             new OpenApiFunctionExecutionParameters()
        //             {
        //                 // EnableDynamicPayload = false,
        //                 EnablePayloadNamespacing = true,
        //                 OperationsToExclude = ["DeleteModel"],
        //             },
        //             cancellationToken: default
        //         );
        // #pragma warning restore SKEXP0040 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        return kernel;
    }
}

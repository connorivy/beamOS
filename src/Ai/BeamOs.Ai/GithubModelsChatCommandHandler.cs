using System.ClientModel;
using System.Text;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using OpenAI;

namespace BeamOs.Ai;

public class GithubModelsChatCommandHandler(
    CreateModelProposalCommandHandler createModelProposalCommandHandler,
    IQueryHandler<Guid, ModelInfoResponse> modelInfoQueryHandler
) : ICommandHandler<GithubModelsChatCommand, GithubModelsChatResponse>
{
    public async Task<Result<GithubModelsChatResponse>> ExecuteAsync(
        GithubModelsChatCommand command,
        CancellationToken ct = default
    )
    {
        var modelInfoTask = modelInfoQueryHandler.ExecuteAsync(command.ModelId, ct);
        var aiApiPlugin = new AiApiPlugin();
        var kernel = this.BuildOpenAiKernel(command.ApiKey, aiApiPlugin);
        var modelInfoResult = await modelInfoTask;
        if (modelInfoResult.IsError)
        {
            return modelInfoResult.Error;
        }

        var agent = new ChatCompletionAgent()
        {
            Instructions =
                $@"
                Your job is to make changes to and answer questions about structural analysis models based off of user requests.

                These are the default units for the model:
                - Length: {modelInfoResult.Value.Settings.UnitSettings.LengthUnit}
                - Force: {modelInfoResult.Value.Settings.UnitSettings.ForceUnit}
                - Angle: {modelInfoResult.Value.Settings.UnitSettings.AngleUnit}
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
            new() { ModelId = command.ModelId, Body = aiApiPlugin.GetModelProposalData() },
            ct
        );

        if (proposalResult.IsError)
        {
            return proposalResult.Error;
        }

        return new GithubModelsChatResponse()
        {
            ProposalId = proposalResult.Value.Id,
            Message = stringBuilder.ToString(),
        };
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

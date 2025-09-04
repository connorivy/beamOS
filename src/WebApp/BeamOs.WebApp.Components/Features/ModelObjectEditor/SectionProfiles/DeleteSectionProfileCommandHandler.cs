using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.WebApp.Components.Features.Common;
using BeamOs.WebApp.Components.Features.Editor;
using BeamOs.WebApp.EditorCommands;
using Fluxor;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace BeamOs.WebApp.Components.Features.ModelObjectEditor.SectionProfiles;

public sealed class DeleteSectionProfileCommandHandler(
    ILogger<DeleteSectionProfileCommandHandler> logger,
    ISnackbar snackbar,
    IStructuralAnalysisApiClientV1 structuralAnalysisApiClientV1,
    IDispatcher dispatcher,
    IState<EditorComponentState> editorState
)
    : ClientCommandHandlerBase<DeleteSectionProfileClientCommand, ModelEntityResponse>(
        logger,
        snackbar
    )
{
    protected override async ValueTask<Result<ModelEntityResponse>> UpdateServer(
        DeleteSectionProfileClientCommand command,
        CancellationToken ct = default
    )
    {
        return await structuralAnalysisApiClientV1.DeleteSectionProfile(
            command.ModelId,
            command.SectionProfileId,
            ct
        );
    }

    protected override ValueTask<Result> UpdateClient(
        DeleteSectionProfileClientCommand command,
        Result<ModelEntityResponse> serverResponse
    )
    {
        if (serverResponse.IsSuccess)
        {
            dispatcher.Dispatch(command);
        }

        return new(Result.Success);
    }
}

public sealed class DeleteSectionProfileSimpleCommandHandler(
    DeleteSectionProfileCommandHandler deleteSectionProfileCommandHandler,
    IState<EditorComponentState> editorState
)
    : SimpleCommandHandlerBase<
        ModelEntityCommand,
        DeleteSectionProfileClientCommand,
        ModelEntityResponse
    >(deleteSectionProfileCommandHandler)
{
    protected override DeleteSectionProfileClientCommand CreateCommand(
        ModelEntityCommand simpleCommand
    )
    {
        var sectionProfile =
            (
                editorState.Value.CachedModelResponse?.SectionProfiles.GetValueOrDefault(
                    simpleCommand.Id
                )
            ) ?? throw new InvalidOperationException("Section profile not found in editor state");

        return new()
        {
            ModelId = simpleCommand.ModelId,
            SectionProfileId = simpleCommand.Id,
            Data = sectionProfile.ToSectionProfileData(),
        };
    }
}

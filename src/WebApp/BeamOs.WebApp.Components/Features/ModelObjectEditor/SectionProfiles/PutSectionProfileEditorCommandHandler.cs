using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfile;
using BeamOs.WebApp.Components.Features.Common;
using BeamOs.WebApp.Components.Features.Editor;
using BeamOs.WebApp.EditorCommands;
using Fluxor;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace BeamOs.WebApp.Components.Features.ModelObjectEditor.SectionProfiles;

public sealed class PutSectionProfileEditorCommandHandler(
    ILogger<PutSectionProfileCommandHandler> logger,
    ISnackbar snackbar,
    IStructuralAnalysisApiClientV1 structuralAnalysisApiClientV1,
    IDispatcher dispatcher
)
    : ClientCommandHandlerBase<PutSectionProfileClientCommand, SectionProfileResponse>(
        logger,
        snackbar
    )
{
    protected override async ValueTask<Result<SectionProfileResponse>> UpdateServer(
        PutSectionProfileClientCommand command,
        CancellationToken ct = default
    )
    {
        return await structuralAnalysisApiClientV1.PutSectionProfileAsync(
            command.New.Id,
            command.New.ModelId,
            command.New.ToSectionProfileData(),
            ct
        );
    }

    protected override ValueTask<Result> UpdateClient(
        PutSectionProfileClientCommand command,
        Result<SectionProfileResponse> serverResponse
    )
    {
        if (serverResponse.IsSuccess)
        {
            dispatcher.Dispatch(command);
        }

        return new(Result.Success);
    }
}

public sealed class PutSectionProfileSimpleCommandHandler(
    PutSectionProfileEditorCommandHandler putSectionProfileEditorCommandHandler,
    IState<EditorComponentState> editorState
)
    : SimpleCommandHandlerBase<
        PutSectionProfileCommand,
        PutSectionProfileClientCommand,
        SectionProfileResponse
    >(putSectionProfileEditorCommandHandler)
{
    protected override PutSectionProfileClientCommand CreateCommand(
        PutSectionProfileCommand simpleCommand
    )
    {
        var sectionProfile =
            (
                editorState
                    .Value
                    .CachedModelResponse
                    ?.SectionProfiles
                    .GetValueOrDefault(simpleCommand.Id)
            ) ?? throw new InvalidOperationException("Section profile not found in editor state");

        return new(sectionProfile, simpleCommand.ToResponse());
    }
}

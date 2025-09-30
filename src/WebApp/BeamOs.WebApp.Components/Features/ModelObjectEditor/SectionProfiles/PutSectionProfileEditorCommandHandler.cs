using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles;
using BeamOs.WebApp.Components.Features.Common;
using BeamOs.WebApp.Components.Features.Editor;
using Fluxor;
using Microsoft.Extensions.Logging;
using MudBlazor;
using Riok.Mapperly.Abstractions;

namespace BeamOs.WebApp.Components.Features.ModelObjectEditor.SectionProfiles;

public sealed class PutSectionProfileEditorCommandHandler(
    ILogger<PutSectionProfileEditorCommandHandler> logger,
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
            command.New.ModelId,
            command.New.Id,
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
        ModelResourceWithIntIdRequest<SectionProfileData>,
        PutSectionProfileClientCommand,
        SectionProfileResponse
    >(putSectionProfileEditorCommandHandler)
{
    protected override PutSectionProfileClientCommand CreateCommand(
        ModelResourceWithIntIdRequest<SectionProfileData> simpleCommand
    )
    {
        var sectionProfile =
            (
                editorState.Value.CachedModelResponse?.SectionProfiles.GetValueOrDefault(
                    simpleCommand.Id
                )
            ) ?? throw new InvalidOperationException("Section profile not found in editor state");

        return new(sectionProfile, simpleCommand.ToResponse());
    }
}

[Mapper]
public static partial class SectionProfileMappers
{
    [MapNestedProperties(nameof(ModelResourceWithIntIdRequest<>.Body))]
    public static partial SectionProfileResponse ToResponse(
        this ModelResourceWithIntIdRequest<SectionProfileData> command
    );
}

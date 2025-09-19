using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles;
using BeamOs.WebApp.EditorCommands.Interfaces;

namespace BeamOs.WebApp.Components.Features.ModelObjectEditor.SectionProfiles;

public record PutSectionProfileClientCommand(
    SectionProfileResponse Previous,
    SectionProfileResponse New
) : IBeamOsUndoableClientCommand
{
    public Guid Id { get; } = Guid.NewGuid();

    public bool HandledByEditor { get; init; }
    public bool HandledByBlazor { get; init; }
    public bool HandledByServer { get; init; }

    public IBeamOsUndoableClientCommand GetUndoCommand(BeamOsClientCommandArgs? args = null) =>
        new PutSectionProfileClientCommand(this.New, this.Previous)
        {
            HandledByEditor = this.HandledByEditor,
            HandledByBlazor = this.HandledByBlazor,
            HandledByServer = this.HandledByServer,
        };

    // public PutSectionProfileCommand ToServerCommand()
    // {
    //     return new PutSectionProfileCommand
    //     {
    //         Id = this.New.Id,
    //         ModelId = this.New.ModelId,
    //         Body =
    //     };
    // }

    public IBeamOsUndoableClientCommand WithArgs(BeamOsClientCommandArgs? args = null) =>
        this with
        {
            HandledByEditor = args?.HandledByEditor ?? this.HandledByEditor,
            HandledByBlazor = args?.HandledByBlazor ?? this.HandledByBlazor,
            HandledByServer = args?.HandledByServer ?? this.HandledByServer,
        };
}

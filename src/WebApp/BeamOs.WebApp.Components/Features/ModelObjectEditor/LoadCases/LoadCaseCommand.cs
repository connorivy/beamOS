using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCases;
using BeamOs.WebApp.EditorCommands.Interfaces;

namespace BeamOs.WebApp.Components.Features.ModelObjectEditor.LoadCases;

public record PutLoadCaseClientCommand(Guid ModelId, LoadCase Previous, LoadCase New)
    : IBeamOsClientCommand
{
    public Guid Id { get; } = Guid.NewGuid();

    public bool HandledByEditor { get; init; }
    public bool HandledByBlazor { get; init; }
    public bool HandledByServer { get; init; }

    public IBeamOsClientCommand GetUndoCommand(BeamOsClientCommandArgs? args = null) =>
        new PutLoadCaseClientCommand(this.ModelId, this.New, this.Previous)
        {
            HandledByEditor = this.HandledByEditor,
            HandledByBlazor = this.HandledByBlazor,
            HandledByServer = this.HandledByServer,
        };

    // public PutLoadCaseCommand ToServerCommand()
    // {
    //     return new PutLoadCaseCommand
    //     {
    //         Id = this.New.Id,
    //         ModelId = this.New.ModelId,
    //         Body =
    //     };
    // }

    public IBeamOsClientCommand WithArgs(BeamOsClientCommandArgs? args = null) =>
        this with
        {
            HandledByEditor = args?.HandledByEditor ?? this.HandledByEditor,
            HandledByBlazor = args?.HandledByBlazor ?? this.HandledByBlazor,
            HandledByServer = args?.HandledByServer ?? this.HandledByServer,
        };
}

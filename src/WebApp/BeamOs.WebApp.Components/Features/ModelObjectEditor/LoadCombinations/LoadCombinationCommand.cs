using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCombinations;
using BeamOs.WebApp.EditorCommands.Interfaces;

namespace BeamOs.WebApp.Components.Features.ModelObjectEditor.LoadCombinations;

public record PutLoadCombinationClientCommand(
    Guid ModelId,
    LoadCombination Previous,
    LoadCombination New
) : IBeamOsClientCommand
{
    public Guid Id { get; } = Guid.NewGuid();

    public bool HandledByEditor { get; init; }
    public bool HandledByBlazor { get; init; }
    public bool HandledByServer { get; init; }

    public IBeamOsClientCommand GetUndoCommand(BeamOsClientCommandArgs? args = null) =>
        new PutLoadCombinationClientCommand(this.ModelId, this.New, this.Previous)
        {
            HandledByEditor = this.HandledByEditor,
            HandledByBlazor = this.HandledByBlazor,
            HandledByServer = this.HandledByServer,
        };

    // public PutLoadCombinationCommand ToServerCommand()
    // {
    //     return new PutLoadCombinationCommand
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

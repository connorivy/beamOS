using BeamOs.WebApp.EditorEvents;

namespace BeamOs.WebApp.EditorActionsAndEvents.Nodes;

public readonly record struct NodeMovedEvent : IUndoable, IEditorAction
{
    public required string NodeId { get; init; }
    public required EditorLocation PreviousLocation { get; init; }
    public required EditorLocation NewLocation { get; init; }
    public bool UiAlreadyUpdated { get; init; }
    public bool IsUndoAction { get; init; }
    public bool IsRedoAction { get; init; }

    public IUndoable GetUndoAction() =>
        this with
        {
            NewLocation = this.PreviousLocation,
            PreviousLocation = this.NewLocation
        };
}

public readonly record struct EditorLocation
{
    public required double XCoordinate { get; init; }
    public required double YCoordinate { get; init; }
    public required double ZCoordinate { get; init; }
}

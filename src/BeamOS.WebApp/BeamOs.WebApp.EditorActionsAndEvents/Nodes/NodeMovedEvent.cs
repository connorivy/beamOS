namespace BeamOs.WebApp.EditorActionsAndEvents.Nodes;

public readonly record struct NodeMovedEvent
{
    public required string NodeId { get; init; }
    public required EditorLocation PreviousLocation { get; init; }
    public required EditorLocation NewLocation { get; init; }

    public void Undo(Action<object> dispatcher)
    {
        dispatcher(
            this with
            {
                NewLocation = this.PreviousLocation,
                PreviousLocation = this.NewLocation
            }
        );
    }

    public void Redo(Action<object> dispatcher)
    {
        dispatcher(this);
    }
}

public readonly record struct EditorLocation
{
    public required double XCoordinate { get; init; }
    public required double YCoordinate { get; init; }
    public required double ZCoordinate { get; init; }
}

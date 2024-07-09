namespace BeamOs.Common.Events;

public interface IUndoable : IIntegrationEvent
{
    public IUndoable GetUndoAction();
}

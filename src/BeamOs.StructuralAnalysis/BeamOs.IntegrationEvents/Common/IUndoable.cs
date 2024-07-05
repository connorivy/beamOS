namespace BeamOs.IntegrationEvents.Common;

public interface IUndoable : IIntegrationEvent
{
    public IUndoable GetUndoAction();
}

namespace BeamOs.Common.Events;

public interface IUndoable : IDomainEvent
{
    public IUndoable GetUndoAction();
}

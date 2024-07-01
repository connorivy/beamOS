namespace BeamOs.WebApp.EditorEvents;


//public abstract record UndoAction : IUndoAction
//{
//    public bool IsUndoAction { get; }
//    public bool IsRedoAction { get; }

//    public void Undo(Action<object> dispatcher)
//    {

//    }

//    public abstract UndoAction GetUndoAction();
//}

//internal readonly record struct UndoAction : IUndoable
//{
//    public IUndoable ActionBeingUndone { get; }
//    public bool IsUndoAction { get; init; }

//    public UndoAction(IUndoable actionBeingUndone)
//    {
//        this.ActionBeingUndone = actionBeingUndone;
//    }

//    public IUndoable GetUndoAction() => this.ActionBeingUndone;
//}

//internal readonly record struct RedoAction : IUndoable
//{
//    public IUndoable ActionBeingRedone { get; }
//    public bool IsUndoAction { get; init; }

//    public RedoAction(IUndoable actionBeingRedone)
//    {
//        this.ActionBeingRedone = actionBeingRedone;
//    }

//    public IUndoable GetUndoAction() => this.ActionBeingRedone.GetUndoAction();
//}

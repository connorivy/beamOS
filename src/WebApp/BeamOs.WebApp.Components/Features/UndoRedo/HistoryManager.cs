using BeamOs.WebApp.Components.Features.Common;
using BeamOs.WebApp.EditorCommands.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOs.WebApp.Components.Features.UndoRedo;

public sealed class HistoryManager(IServiceProvider serviceProvider)
{
    private readonly LinkedList<IBeamOsClientCommand> undoActions = new();
    private readonly LinkedList<IBeamOsClientCommand> redoActions = new();
    private readonly int itemLimit = 50;

    public async Task UndoLast()
    {
        if (this.undoActions.FirstOrDefault() is not IBeamOsClientCommand undoable)
        {
            // no undo history
            return;
        }
        var command = undoable.GetUndoCommand(BeamOsClientCommandArgs.Unhandled);
        var commandType = command.GetType();
        var commandHandlerType = typeof(IClientCommandHandler<>).MakeGenericType(commandType);
        var commandHandler = (IClientCommandHandler)
            serviceProvider.GetRequiredService(commandHandlerType);

        await commandHandler.ExecuteAsync(command, CancellationToken.None);

        this.undoActions.RemoveFirst();
        this.redoActions.AddFirst(undoable);
    }

    public async Task Redo()
    {
        if (this.redoActions.FirstOrDefault() is not IBeamOsClientCommand undoable)
        {
            // no redo history
            return;
        }

        var command = undoable.WithArgs(BeamOsClientCommandArgs.Unhandled);
        var commandType = command.GetType();
        var commandHandlerType = typeof(IClientCommandHandler<>).MakeGenericType(commandType);
        var commandHandler = (IClientCommandHandler)
            serviceProvider.GetRequiredService(commandHandlerType);

        await commandHandler.ExecuteAsync(command, CancellationToken.None);

        this.redoActions.RemoveFirst();
        this.undoActions.AddFirst(undoable);
    }

    public void AddItem(IBeamOsClientCommand clientEvent)
    {
        if (clientEvent.Id == this.undoActions.FirstOrDefault()?.Id)
        {
            // this is undo event
        }
        else if (clientEvent.Id == this.redoActions.FirstOrDefault()?.Id)
        {
            // this is redo event
        }
        else
        {
            // this is brand new event
            this.undoActions.AddFirst(clientEvent);

            if (this.redoActions.Count > 0)
            {
                // doing a new action will clear the list of redo actions
                this.redoActions.Clear();
            }
        }
    }

    public void Clear()
    {
        this.undoActions.Clear();
        this.redoActions.Clear();
    }
}

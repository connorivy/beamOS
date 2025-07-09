using BeamOs.Common.Contracts;
using BeamOs.WebApp.EditorCommands.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BeamOs.WebApp.Components.Features.StructuralApi;

public readonly record struct ModelEntityCreated : IBeamOsUndoableClientCommand
{
    public Guid Id { get; }
    public bool HandledByEditor { get; init; }
    public bool HandledByBlazor { get; init; }
    public bool HandledByServer { get; init; }
    public IModelEntity ModelEntity { get; init; }

    public IBeamOsUndoableClientCommand GetUndoCommand(BeamOsClientCommandArgs? args = null) =>
        new ModelEntityDeleted() { ModelEntity = this.ModelEntity };

    public IBeamOsUndoableClientCommand WithArgs(BeamOsClientCommandArgs? args = null) =>
        throw new NotImplementedException();
}

public readonly record struct ModelEntitiesCreated : IBeamOsUndoableClientCommand
{
    public Guid Id { get; }
    public bool HandledByEditor { get; init; }
    public bool HandledByBlazor { get; init; }
    public bool HandledByServer { get; init; }
    public ICollection<IModelEntity> ModelEntity { get; init; }

    public IBeamOsUndoableClientCommand GetUndoCommand(BeamOsClientCommandArgs? args = null) =>
        //new ModelEntityDeleted() { ModelEntity = this.ModelEntity };
        throw new NotImplementedException();

    public IBeamOsUndoableClientCommand WithArgs(BeamOsClientCommandArgs? args = null) =>
        throw new NotImplementedException();
}

public readonly record struct ModelEntityUpdated
//: IBeamOsClientCommand
{
    public Guid Id { get; }
    public bool HandledByEditor { get; init; }
    public bool HandledByBlazor { get; init; }
    public bool HandledByServer { get; init; }
    public IModelEntity ModelEntity { get; init; }
    public IModelEntity PreviousModelEntity { get; init; }

    //public IBeamOsClientCommand GetUndoCommand(BeamOsClientCommandArgs? args = null) =>
    //    new ModelEntityUpdated()
    //    {
    //        ModelEntity = this.PreviousModelEntity,
    //        PreviousModelEntity = this.ModelEntity
    //    };

    //public IBeamOsClientCommand WithArgs(BeamOsClientCommandArgs? args = null) =>
    //    throw new NotImplementedException();
}

public readonly record struct ModelEntityDeleted : IBeamOsUndoableClientCommand
{
    public Guid Id { get; }
    public bool HandledByEditor { get; init; }
    public bool HandledByBlazor { get; init; }
    public bool HandledByServer { get; init; }
    public IModelEntity ModelEntity { get; init; }
    public string EntityType { get; init; }

    public IBeamOsUndoableClientCommand GetUndoCommand(BeamOsClientCommandArgs? args = null) =>
        new ModelEntityCreated() { ModelEntity = this.ModelEntity };

    public IBeamOsUndoableClientCommand WithArgs(BeamOsClientCommandArgs? args = null) =>
        throw new NotImplementedException();
}

public readonly record struct ElementReferencesEvaluated(List<ElementReference> ElementReferences);

public readonly record struct ApiClientMethodSelected(
    Guid ModelId,
    ApiClientMethodInfo? MethodInfo
);

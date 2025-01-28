namespace BeamOs.Common.Contracts;

public interface IHasModelId
{
    public Guid ModelId { get; }
}

public interface IModelEntity : IHasModelId, IBeamOsEntityResponse
{
    public int Id { get; }
}

public interface IBeamOsEntityResponse { }

namespace BeamOs.Common.Contracts;

public interface IHasModelId
{
    public Guid ModelId { get; }
}

public interface IModelEntity : IHasModelId
{
    public int Id { get; }
}

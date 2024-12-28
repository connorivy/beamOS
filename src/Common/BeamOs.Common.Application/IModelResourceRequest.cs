namespace BeamOs.Common.Application;

public interface IHasModelId
{
    public Guid ModelId { get; }
}

public interface IModelResourceRequest<TBody> : IHasModelId
{
    public new Guid ModelId { get; init; }
    public TBody Body { get; init; }
}

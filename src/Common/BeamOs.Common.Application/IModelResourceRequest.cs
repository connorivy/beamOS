namespace BeamOs.Common.Application;

public interface IModelResourceRequest<TBody>
{
    public Guid ModelId { get; init; }
    public TBody Body { get; init; }
}

namespace BeamOs.StructuralAnalysis.Application.Common;

public interface IModelResourceRequest<TBody>
{
    public Guid ModelId { get; init; }
    public TBody Body { get; init; }
}

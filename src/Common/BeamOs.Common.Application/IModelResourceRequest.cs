using BeamOs.Common.Contracts;

namespace BeamOs.Common.Application;

public interface IModelResourceRequest<TBody> : IHasModelId
{
    public new Guid ModelId { get; init; }
    public TBody Body { get; init; }
}

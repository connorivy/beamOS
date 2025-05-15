namespace BeamOs.Common.Contracts;

public interface IModelResourceRequest<TBody> : IHasModelId
{
    public new Guid ModelId { get; init; }
    public TBody Body { get; init; }
}

public interface IModelResourceWithIntIdRequest<TBody> : IModelResourceRequest<TBody>
{
    public int Id { get; init; }
}

public readonly struct ModelResourceRequest<TBody> : IModelResourceRequest<TBody>
{
    public Guid ModelId { get; init; }
    public TBody Body { get; init; }

    public ModelResourceRequest(Guid modelId, TBody body)
    {
        this.ModelId = modelId;
        this.Body = body;
    }
}

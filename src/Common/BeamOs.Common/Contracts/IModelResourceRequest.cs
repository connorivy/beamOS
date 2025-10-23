using System.Diagnostics.CodeAnalysis;

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

public readonly struct ModelResourceRequest(Guid modelId) : IHasModelId
{
    public Guid ModelId { get; init; } = modelId;
}

public readonly struct ModelResourceRequest<TBody> : IModelResourceRequest<TBody>
{
    public Guid ModelId { get; init; }

    [BeamOs.Common.Contracts.FromBody]
    public TBody Body { get; init; }

    public ModelResourceRequest(Guid modelId, TBody body)
    {
        this.ModelId = modelId;
        this.Body = body;
    }
}

public readonly record struct ModelResourceWithIntIdRequest : IModelEntity
{
    [SetsRequiredMembers]
    public ModelResourceWithIntIdRequest(Guid modelId, int id)
    {
        this.ModelId = modelId;
        this.Id = id;
    }

    public ModelResourceWithIntIdRequest() { }

    public required int Id { get; init; }
    public required Guid ModelId { get; init; }
}

public readonly struct ModelResourceWithIntIdRequest<TBody> : IModelResourceWithIntIdRequest<TBody>
{
    public int Id { get; init; }
    public Guid ModelId { get; init; }
    public TBody Body { get; init; }

    public ModelResourceWithIntIdRequest(Guid modelId, int id, TBody body)
    {
        this.ModelId = modelId;
        this.Id = id;
        this.Body = body;
    }
}

public readonly struct EmptyRequest;

public readonly struct GetAnalyticalResultQuery : IHasModelId
{
    public Guid ModelId { get; init; }
    public int LoadCombinationId { get; init; }
}

public readonly struct GetAnalyticalResultResourceQuery : IHasModelId
{
    public Guid ModelId { get; init; }
    public int LoadCombinationId { get; init; }
    public int Id { get; init; }
}

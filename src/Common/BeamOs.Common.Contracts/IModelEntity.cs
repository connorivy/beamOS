using System.Globalization;

namespace BeamOs.Common.Contracts;

public interface IHasModelId
{
    public Guid ModelId { get; }
}

public interface IHasIntId
{
    public int Id { get; }
}

public interface IModelEntity : IHasModelId, IHasIntId, IBeamOsEntityResponse { }

public interface IBeamOsEntityResponse { }

using BeamOs.Common.Contracts;

namespace BeamOs.Common.Application;

public interface IQueryHandler<TQuery, TResponse>
{
    Task<Result<TResponse>> ExecuteAsync(TQuery query, CancellationToken ct = default);
}

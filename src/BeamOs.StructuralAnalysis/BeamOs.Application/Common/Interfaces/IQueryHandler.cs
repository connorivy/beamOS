namespace BeamOs.Application.Common.Interfaces;

public interface IQueryHandler<TQuery, TResponse>
{
    Task<TResponse?> ExecuteAsync(TQuery query, CancellationToken ct = default);
}

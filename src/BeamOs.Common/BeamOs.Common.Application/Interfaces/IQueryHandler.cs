namespace BeamOs.Common.Application.Interfaces;

public interface IQueryHandler<TQuery, TResponse>
{
    Task<TResponse?> ExecuteAsync(TQuery query, CancellationToken ct = default);
}

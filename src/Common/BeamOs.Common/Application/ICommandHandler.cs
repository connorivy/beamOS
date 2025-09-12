using BeamOs.Common.Contracts;

namespace BeamOs.Common.Application;

public interface IBaseCommandHandler<TCommand, TResponse>
{
    public Task<TResponse> ExecuteAsync(TCommand command, CancellationToken ct = default);
}

public interface IAsyncEnumerableCommandHandler<TCommand, TResponse>
{
    public IAsyncEnumerable<TResponse> ExecuteAsync(
        TCommand command,
        CancellationToken ct = default
    );
}

public interface ICommandHandler<TCommand, TResponse>
{
    public Task<Result<TResponse>> ExecuteAsync(TCommand command, CancellationToken ct = default);
}

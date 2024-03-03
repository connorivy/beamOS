namespace BeamOs.Application.Common.Interfaces;

public interface ICommandHandler<TCommand, TResponse>
{
    Task<TResponse> ExecuteAsync(TCommand command, CancellationToken ct = default);
}

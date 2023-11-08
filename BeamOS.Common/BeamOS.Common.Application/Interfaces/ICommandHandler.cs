namespace BeamOS.Common.Application.Interfaces;

public interface ICommandHandler<TCommand, TResponse>
{
    Task<TResponse> ExecuteAsync(TCommand command, CancellationToken ct = default);
}

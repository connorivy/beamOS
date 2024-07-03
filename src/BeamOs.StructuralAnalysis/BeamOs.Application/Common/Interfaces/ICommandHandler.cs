namespace BeamOs.Application.Common.Interfaces;

public interface ICommandHandler<TCommand, TResponse>
{
    Task<TResponse> ExecuteAsync(TCommand command, CancellationToken ct = default);
}

public interface ICommandHandlerSync<TCommand, TResponse>
{
    TResponse Execute(TCommand command);
}

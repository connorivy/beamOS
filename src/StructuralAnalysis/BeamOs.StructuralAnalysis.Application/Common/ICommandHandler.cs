using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.StructuralAnalysis.Application.Common;

public interface ICommandHandler<TCommand, TResponse>
{
    public Task<Result<TResponse>> ExecuteAsync(TCommand command, CancellationToken ct = default);
}

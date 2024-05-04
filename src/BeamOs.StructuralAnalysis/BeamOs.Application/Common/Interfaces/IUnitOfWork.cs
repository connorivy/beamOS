namespace BeamOs.Application.Common.Interfaces;

public interface IUnitOfWork
{
    public Task SaveChangesAsync(CancellationToken ct = default);
}

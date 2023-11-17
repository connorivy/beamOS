using BeamOS.Common.Application.Interfaces;
using BeamOS.PhysicalModel.Domain.ModelAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.ModelAggregate;

namespace BeamOS.PhysicalModel.Infrastructure.Models;

//public class ModelDbContextRepository(PhysicalModelDbContext dbContext) : IRepository<ModelId, Model>
//{
//    public async Task Add(Model aggregate)
//    {
//        _ = await dbContext.Models.AddAsync(aggregate);
//        _ = await dbContext.SaveChangesAsync();
//    }
//    public Task<Model?> GetById(ModelId id)
//    {
//        return Task.FromResult(dbContext.Models
//            .Where(el => el.Id == id)
//            .FirstOrDefault());
//    }
//}

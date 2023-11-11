using BeamOS.Common.Application.Interfaces;
using BeamOS.DirectStiffnessMethod.Domain.AnalyticalModelAggregate;
//using Riok.Mapperly.Abstractions;

//namespace BeamOS.DirectStiffnessMethod.Application.Models.Commands;

//public class CreateAnalyticalModelCommandHandler(IRepository<ModelId, Model> modelRepository)
//    : ICommandHandler<CreateAnalyticalModelCommand, Model>
//{
//    public async Task<AnalyticalModel> ExecuteAsync(CreateAnalyticalModelCommand command, CancellationToken ct = default)
//    {
//        var model = command.ToDomainObject();

//        await modelRepository.Add(model);

//        return model;
//    }
//}

//[Mapper]
//public static partial class CreateModelCommandMapper
//{
//    public static partial Model ToDomainObject(this CreateAnalyticalModelCommand command);
//}

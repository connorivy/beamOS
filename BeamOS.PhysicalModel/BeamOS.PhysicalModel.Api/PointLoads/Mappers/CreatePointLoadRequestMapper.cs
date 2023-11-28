using BeamOS.Common.Api.Interfaces;
using BeamOS.Common.Contracts;
using BeamOS.PhysicalModel.Application.PointLoads.Commands;
using BeamOS.PhysicalModel.Contracts.Common;
using BeamOS.PhysicalModel.Contracts.PointLoad;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Riok.Mapperly.Abstractions;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOS.PhysicalModel.Api.PointLoads.Mappers;

[Mapper]
[UseStaticMapper(typeof(Vector3Mapper))]
[UseStaticMapper(typeof(UnitValueDTOToForceMapper))]
public partial class CreatePointLoadRequestMapper : IMapper<CreatePointLoadRequest, CreatePointLoadCommand>
{
    public CreatePointLoadCommand Map(CreatePointLoadRequest from) => this.ToCommand(from);
    public partial CreatePointLoadCommand ToCommand(CreatePointLoadRequest request);
}

public static class Vector3Mapper
{
    public static Vector<double> MapVector3(this Vector3 vector3)
    {
        return DenseVector.OfArray([vector3.X, vector3.Y, vector3.Z]);
    }

    public static Vector3 MapMathnetVector(this Vector<double> vector)
    {
        return new(vector[0], vector[1], vector[2]);
    }
}

[Mapper]
public static partial class StringToForceUnitMapper
{
    public static partial ForceUnit MapToForceUnit(this string unit);
}

public static class UnitValueDTOToForceMapper
{
    public static Force MapToForce(this UnitValueDTO dto)
    {
        return new(dto.Value, dto.Unit.MapToForceUnit());
    }
}

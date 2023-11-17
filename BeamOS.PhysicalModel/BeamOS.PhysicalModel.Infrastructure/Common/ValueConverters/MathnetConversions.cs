using System;
using BeamOS.Common.Domain.Models;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BeamOS.PhysicalModel.Infrastructure.Common.ValueConverters;

// TODO : this is very not efficient
public class VectorDoubleConverter : ValueConverter<Vector<double>, string>
{
    public VectorDoubleConverter()
    : base(
            x => string.Join(';', x.ToArray()),
            x => DenseVector.OfArray(Array.ConvertAll(x.Split(';', StringSplitOptions.RemoveEmptyEntries), double.Parse)),
            null)
    {
    }
}

//public class Vector : BeamOSValueObject
//{
//    public string Value { get; private set; }

//    public Vector(params double[] doubles)
//    {
//        this.Value = string.Join(';', doubles);
//    }

//    protected override IEnumerable<object> GetEqualityComponents()
//    {
//        yield return Value;
//    }
//}

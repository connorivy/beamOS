using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Spatial.Euclidean;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BeamOs.StructuralAnalysis.Infrastructure.Common;

// TODO : this is very not efficient
internal class VectorDoubleConverter : ValueConverter<Vector<double>, string>
{
    public VectorDoubleConverter()
        : base(
            x => string.Join(';', x.ToArray()),
            x =>
                DenseVector.OfArray(
                    Array.ConvertAll(
                        x.Split(';', StringSplitOptions.RemoveEmptyEntries),
                        double.Parse
                    )
                ),
            null
        ) { }
}

internal class Vector3dConverter : ValueConverter<Vector3D, string>
{
    public Vector3dConverter()
        : base(x => x.ToString(), x => Vector3D.Parse(x, null), null) { }
}

//internal class UnitVector3dConverter : ValueConverter<UnitVector3D, string>
//{
//    public UnitVector3dConverter()
//        : base(x => x.ToString(), x => UnitVector3D.Parse(x, null, .1), null) { }
//}

internal class PolynomialConverter : ValueConverter<Polynomial, IEnumerable<double>>
{
    public PolynomialConverter()
        : base(x => x.Coefficients, x => new(x)) { }
}

//internal class Vector : BeamOSValueObject
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

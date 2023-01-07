using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace beamOS.API.Schema.Objects
{
  public class Element1D
  {
    // Base Curve of element1D. Only lines implemented right now but extendable to curves
    public ICurve BaseCurve { get; set; }
    // counter-clockwise rotation in radians when looking in the negative (local) x direction
    public double ProfileRotation { get; set; }
    public SectionProfile SectionProfile { get; set; }
    public Material Material { get; set; }
    public List<Node> Nodes { get; set; } = new List<Node>();
    public List<Load> Loads { get; set; } = new List<Load>();
    public ElementType ElementType { get; set; }
    public Element1D(ICurve curve, SectionProfile section, Material mat, ElementType type = ElementType.Truss) 
    { 
      BaseCurve = curve;
      SectionProfile = section;
      Material = mat;
      ElementType = type;
    }

    public Matrix<double> CreateStiffnessMatrix()
    {
      // if baseCurve is a line, life is good
      if (BaseCurve is Line baseLine)
      {
        var rotationMatrix = GetRotationMatrix(baseLine);
        var transformationMatrix = GetTransformationMatrix(rotationMatrix);
        GetLocalStiffnessMatrix(baseLine);
      }
      return null;
    }

    public Matrix<double> GetTransformationMatrix(Matrix<double> rotationMatrix)
    {
      if (rotationMatrix.ColumnCount != 3)
        throw new Exception($"The method \"GetTransformationMatrix\" must have 3 columns, not ${rotationMatrix.ColumnCount}");
      if (rotationMatrix.RowCount != 3)
        throw new Exception($"The method \"GetTransformationMatrix\" must have 3 columns, not ${rotationMatrix.RowCount}");

      var transformationMatrix = Matrix<double>.Build.Dense(12, 12);
      transformationMatrix.SetSubMatrix(0, 0, rotationMatrix);
      transformationMatrix.SetSubMatrix(3, 3, rotationMatrix);
      transformationMatrix.SetSubMatrix(6, 6, rotationMatrix);
      transformationMatrix.SetSubMatrix(9, 9, rotationMatrix);

      return transformationMatrix;
    }

    public Matrix<double> GetRotationMatrix(Line baseLine)
    {
      var L = baseLine.Length;

      var rxx = (baseLine.P1[0] - baseLine.P0[0]) / L;
      var rxy = (baseLine.P1[1] - baseLine.P0[1]) / L;
      var rxz = (baseLine.P1[2] - baseLine.P0[2]) / L;

      var cosG = Math.Cos(ProfileRotation);
      var sinG = Math.Sin(ProfileRotation);

      var sqrtRxx2Rxz2 = Math.Sqrt(rxx * rxx + rxz * rxz);

      double r21, r22, r23, r31, r32, r33;

      if (sqrtRxx2Rxz2 < .0001)
      {
        r21 = -rxy * cosG;
        r22 = 0;
        r23 = sinG;
        r31 = rxy * sinG;
        r32 = 0;
        r33 = cosG;
      }
      else
      {
        r21 = (-rxx * rxy * cosG - rxz * sinG) / sqrtRxx2Rxz2;
        r22 = sqrtRxx2Rxz2 * cosG;
        r23 = (-rxy * rxz * cosG + rxx * sinG) / sqrtRxx2Rxz2;
        r31 = (rxx * rxy * sinG - rxz * cosG) / sqrtRxx2Rxz2;
        r32 = -sqrtRxx2Rxz2 * sinG;
        r33 = (rxy * rxz * sinG + rxx * cosG) / sqrtRxx2Rxz2;
      }

      return DenseMatrix.OfArray(new[,] {
        { rxx, rxy, rxz },
        { r21, r22, r23 },
        { r31, r32, r33 },
      });
    }

    public Matrix<double> GetLocalStiffnessMatrix(Line baseLine)
    {
      return null;
    }
  }
}

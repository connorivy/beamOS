using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace beamOS.API.Schema.Objects
{
  public class Element1D
  {
    // Base Curve of element1D. Only lines implemented right now but extendable to curves
    public ICurve BaseCurve { get; set; }
    // counter-clockwise rotation in radians
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
        GetTransformationMatrix(baseLine);
        GetLocalStiffnessMatrix(baseLine);
      }
      return null;
    }

    // Transformation matrix will be a 12x12 matrix made up of four instances of a 3x3 matrix defined as:
    // | r11 r12 r13 |   | cos(a)cos(B) cos(a)sin(B)sin(g)-sin(a)cos(g) cos(a)sin(B)cos(g)+sin(a)sin(g) |
    // | r21 r22 r23 | = | sin(a)cos(B) sin(a)sin(B)sin(g)+cos(a)cos(g) sin(a)sin(B)cos(g)-cos(a)sin(g) |
    // | r31 r32 r33 |   |    -sin(B)            cos(B)sin(g)                    cos(B)cos(g)           |
    //
    // where a,B,g are alpha beta and gamma from the Euler YZX convention
    public Matrix<double> GetTransformationMatrix(Line baseLine)
    {
      var L = baseLine.Length;

      //var dX = baseLine.P0[0] - baseLine.P1[0];
      //var dY = baseLine.P0[1] - baseLine.P1[1];
      //var dZ = baseLine.P0[2] - baseLine.P1[2];
      //var cosG = Math.Cos(ProfileRotation);
      //var sinG = Math.Sin(ProfileRotation);
      //var den = L * Math.Sqrt(dX * dX + dY * dY);

      //var r11 = (dX * dZ * cosG - L * dY * sinG) / den;
      //var r12 = (dY * dZ * cosG + L * dX * sinG) / den;
      //var r13 = -den * cosG / (L * L);
      //var r21 = -(dX * dZ * sinG + L * dY * cosG) / den;
      //var r22 = -(dY * dZ * sinG - L * dX * cosG) / den;
      //var r23 = den * sinG / (L * L);
      //var r31 = dX / L;
      //var r32 = dY / L;
      //var r33 = dZ / L;

      //return SparseMatrix.OfArray(new[,] {
      //  { r11, r12, r13, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
      //  { r21, r22, r23, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
      //  { r31, r32, r33, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
      //  { 0, 0, 0, r11, r12, r13, 0, 0, 0, 0, 0, 0 },
      //  { 0, 0, 0, r21, r22, r23, 0, 0, 0, 0, 0, 0 },
      //  { 0, 0, 0, r31, r32, r33, 0, 0, 0, 0, 0, 0 },
      //  { 0, 0, 0, 0, 0, 0, r11, r12, r13, 0, 0, 0 },
      //  { 0, 0, 0, 0, 0, 0, r21, r22, r23, 0, 0, 0 },
      //  { 0, 0, 0, 0, 0, 0, r31, r32, r33, 0, 0, 0 },
      //  { 0, 0, 0, 0, 0, 0, 0, 0, 0, r11, r12, r13 },
      //  { 0, 0, 0, 0, 0, 0, 0, 0, 0, r21, r22, r23 },
      //  { 0, 0, 0, 0, 0, 0, 0, 0, 0, r31, r32, r33 },
      //});


      var rxx = baseLine.P1[0] - baseLine.P0[0] / L;
      var rxy = baseLine.P1[1] - baseLine.P0[1] / L;
      var rxz = baseLine.P1[2] - baseLine.P0[2] / L;

      var sqrtRxx2Rxz2 = Math.Sqrt(rxx * rxx + rxz * rxz);
      var cosG = Math.Cos(ProfileRotation);
      var sinG = Math.Sin(ProfileRotation);

      var r21 = (-rxx * rxy * cosG - rxz * sinG) / sqrtRxx2Rxz2;
      var r22 = sqrtRxx2Rxz2 * cosG;
      var r23 = (-rxx * rxz * cosG + rxx * sinG) / sqrtRxx2Rxz2;
      var r31 = (rxx * rxy * sinG - rxz * cosG) / sqrtRxx2Rxz2;
      var r32 = -sqrtRxx2Rxz2 * sinG;
      var r33 = (rxy * rxz * sinG + rxx * cosG) / sqrtRxx2Rxz2;

      return SparseMatrix.OfArray(new[,] {
        { rxx, rxy, rxz, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { r21, r22, r23, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { r31, r32, r33, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, rxx, rxy, rxz, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, r21, r22, r23, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, r31, r32, r33, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, rxx, rxy, rxz, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, r21, r22, r23, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, r31, r32, r33, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0, rxx, rxy, rxz },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0, r21, r22, r23 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0, r31, r32, r33 },
      });
    }

    public Matrix<double> GetRotationMatrix(Line baseLine)
    {
      var L = baseLine.Length;

      var rxx = (baseLine.P1[0] - baseLine.P0[0]) / L;
      var rxy = (baseLine.P1[1] - baseLine.P0[1]) / L;
      var rxz = (baseLine.P1[2] - baseLine.P0[2]) / L;

      var sqrtRxx2Rxz2 = Math.Sqrt(rxx * rxx + rxz * rxz);
      var cosG = Math.Cos(ProfileRotation);
      var sinG = Math.Sin(ProfileRotation);

      var r21 = (-rxx * rxy * cosG - rxz * sinG) / sqrtRxx2Rxz2;
      var r22 = sqrtRxx2Rxz2 * cosG;
      var r23 = (-rxx * rxz * cosG + rxx * sinG) / sqrtRxx2Rxz2;
      var r31 = (rxx * rxy * sinG - rxz * cosG) / sqrtRxx2Rxz2;
      var r32 = -sqrtRxx2Rxz2 * sinG;
      var r33 = (rxy * rxz * sinG + rxx * cosG) / sqrtRxx2Rxz2;

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

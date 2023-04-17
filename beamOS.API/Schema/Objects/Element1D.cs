namespace beamOS.API.Schema.Objects;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Speckle.Newtonsoft.Json;

public class Element1D : Base<Element1D>
{
  public Element1D() { }
  public int Id { get; set; }
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
    this.BaseCurve = curve;
    this.SectionProfile = section;
    this.Material = mat;
    this.ElementType = type;
  }

  [JsonIgnore]
  [ClearOnModelUnlock]
  public Matrix<double>? localStiffnessMatrix = null;
  [JsonIgnore]
  public Matrix<double> LocalStiffnessMatrix
  {
    get
    {
      if (this.localStiffnessMatrix != null)
      {
        return this.localStiffnessMatrix;
      }

      if (this.Material == null)
      {
        throw new Exception(nameof(this.Material));
      }

      if (this.SectionProfile == null)
      {
        throw new Exception(nameof(this.SectionProfile));
      }

      // TODO: support units
#pragma warning disable IDE1006 // Naming Styles
      var E = this.Material.E;
      var G = this.Material.G;
      var A = this.SectionProfile.A;
      var L = this.BaseCurve.Length;
      var Iz = this.SectionProfile.Iz;
      var Iy = this.SectionProfile.Iy;
      var J = this.SectionProfile.J;
      var L2 = L * L;
      var L3 = L2 * L;
#pragma warning restore IDE1006 // Naming Styles

      this.localStiffnessMatrix = DenseMatrix.OfArray(new[,]
      {
        {  E*A/L,           0,           0,      0,          0,          0, -E*A/L,           0,           0,      0,          0,          0 },
        {      0,  12*E*Iz/L3,           0,      0,          0,  6*E*Iz/L2,      0, -12*E*Iz/L3,           0,      0,          0,  6*E*Iz/L2 },
        {      0,           0,  12*E*Iy/L3,      0, -6*E*Iy/L2,          0,      0,           0, -12*E*Iy/L3,      0, -6*E*Iy/L2,          0 },
        {      0,           0,           0,  G*J/L,          0,          0,      0,           0,           0, -G*J/L,          0,          0 },
        {      0,           0,  -6*E*Iy/L2,      0,   4*E*Iy/L,          0,      0,           0,   6*E*Iy/L2,      0,   2*E*Iy/L,          0 },
        {      0,   6*E*Iz/L2,           0,      0,          0,   4*E*Iz/L,      0,  -6*E*Iz/L2,           0,      0,          0,   2*E*Iz/L },
        { -E*A/L,           0,           0,      0,          0,          0,  E*A/L,           0,           0,      0,          0,          0 },
        {      0, -12*E*Iz/L3,           0,      0,          0, -6*E*Iz/L2,      0,  12*E*Iz/L3,           0,      0,          0, -6*E*Iz/L2 },
        {      0,           0, -12*E*Iy/L3,      0,  6*E*Iy/L2,          0,      0,           0,  12*E*Iy/L3,      0,  6*E*Iy/L2,          0 },
        {      0,           0,           0, -G*J/L,          0,          0,      0,           0,           0,  G*J/L,          0,          0 },
        {      0,           0,  -6*E*Iy/L2,      0,   2*E*Iy/L,          0,      0,           0,   6*E*Iy/L2,      0,   4*E*Iy/L,          0 },
        {      0,   6*E*Iz/L2,           0,      0,          0,   2*E*Iz/L,      0,  -6*E*Iz/L2,           0,      0,          0,   4*E*Iz/L },
      });

      return this.localStiffnessMatrix;
    }
  }

<<<<<<< HEAD
  [JsonIgnore]
  [ClearOnModelUnlock]
  private Matrix<double>? globalStiffnessMatrix = null;
  [JsonIgnore]
  public Matrix<double> GlobalStiffnessMatrix
  {
    get
    {
      if (this.globalStiffnessMatrix != null)
      {
        return this.globalStiffnessMatrix;
      }

      var rotationMatrix = this.GetRotationMatrix();
      var transformationMatrix = this.GetTransformationMatrix(rotationMatrix);

      this.globalStiffnessMatrix = transformationMatrix.Transpose() * this.LocalStiffnessMatrix * transformationMatrix;
      return this.globalStiffnessMatrix;
=======
    private Matrix<double>? _localStiffnessMatrix = null;
    public Matrix<double> LocalStiffnessMatrix
    {
      get
      {
        if (_localStiffnessMatrix != null) 
          return _localStiffnessMatrix;

        if (Material == null)
          throw new NullReferenceException(nameof(Material));

        if (SectionProfile == null)
          throw new NullReferenceException(nameof(SectionProfile));

        // TODO: support units
        var E = Material.E;
        var G = Material.G;
        var A = SectionProfile.A;
        var L = BaseCurve.Length;
        var Iz = SectionProfile.Iz;
        var Iy = SectionProfile.Iy;
        var J = SectionProfile.J;
        var L2 = L * L;
        var L3 = L2 * L;

        _localStiffnessMatrix = DenseMatrix.OfArray(new[,]
        {
          {  E*A/L,           0,           0,      0,          0,          0, -E*A/L,           0,           0,      0,          0,          0 },
          {      0,  12*E*Iz/L3,           0,      0,          0,  6*E*Iz/L2,      0, -12*E*Iz/L3,           0,      0,          0,  6*E*Iz/L2 },
          {      0,           0,  12*E*Iy/L3,      0, -6*E*Iy/L2,          0,      0,           0, -12*E*Iy/L3,      0, -6*E*Iy/L2,          0 },
          {      0,           0,           0,  G*J/L,          0,          0,      0,           0,           0, -G*J/L,          0,          0 },
          {      0,           0,  -6*E*Iy/L2,      0,   4*E*Iy/L,          0,      0,           0,   6*E*Iy/L2,      0,   2*E*Iy/L,          0 },
          {      0,   6*E*Iz/L2,           0,      0,          0,   4*E*Iz/L,      0,  -6*E*Iz/L2,           0,      0,          0,   2*E*Iz/L },
          { -E*A/L,           0,           0,      0,          0,          0,  E*A/L,           0,           0,      0,          0,          0 },
          {      0, -12*E*Iz/L3,           0,      0,          0, -6*E*Iz/L2,      0,  12*E*Iz/L3,           0,      0,          0, -6*E*Iz/L2 },
          {      0,           0, -12*E*Iy/L3,      0,  6*E*Iy/L2,          0,      0,           0,  12*E*Iy/L3,      0,  6*E*Iy/L2,          0 },
          {      0,           0,           0, -G*J/L,          0,          0,      0,           0,           0,  G*J/L,          0,          0 },
          {      0,           0,  -6*E*Iy/L2,      0,   2*E*Iy/L,          0,      0,           0,   6*E*Iy/L2,      0,   4*E*Iy/L,          0 },
          {      0,   6*E*Iz/L2,           0,      0,          0,   2*E*Iz/L,      0,  -6*E*Iz/L2,           0,      0,          0,   4*E*Iz/L },
        });

        return _localStiffnessMatrix;
      }
    }

    private Matrix<double>? _globalStiffnessMatrix = null;
    public Matrix<double> GlobalStiffnessMatrix
    {
      get
      {
        if (_globalStiffnessMatrix != null)
          return _globalStiffnessMatrix;

        if (BaseCurve is not Line baseLine)
          throw new NotSupportedException("Curved elements are not supported yet");

        var rotationMatrix = GetRotationMatrix(baseLine);
        var transformationMatrix = GetTransformationMatrix(rotationMatrix);

        _globalStiffnessMatrix = transformationMatrix.Transpose() * LocalStiffnessMatrix * transformationMatrix;
        return _globalStiffnessMatrix;
      }
>>>>>>> main
    }
  }

  public Matrix<double> GetTransformationMatrix(Matrix<double> rotationMatrix)
  {
    if (rotationMatrix.ColumnCount != 3)
    {
<<<<<<< HEAD
      throw new Exception($"The provided rotation matrix must have 3 columns, not ${rotationMatrix.ColumnCount}");
=======
      if (rotationMatrix.ColumnCount != 3)
        throw new Exception($"The provided rotation matrix must have 3 columns, not ${rotationMatrix.ColumnCount}");
      if (rotationMatrix.RowCount != 3)
        throw new Exception($"The provided rotation matrix must have 3 rows, not ${rotationMatrix.RowCount}");

      var transformationMatrix = Matrix<double>.Build.Dense(12, 12);
      transformationMatrix.SetSubMatrix(0, 0, rotationMatrix);
      transformationMatrix.SetSubMatrix(3, 3, rotationMatrix);
      transformationMatrix.SetSubMatrix(6, 6, rotationMatrix);
      transformationMatrix.SetSubMatrix(9, 9, rotationMatrix);

      return transformationMatrix;
>>>>>>> main
    }

    if (rotationMatrix.RowCount != 3)
    {
      throw new Exception($"The provided rotation matrix must have 3 rows, not ${rotationMatrix.RowCount}");
    }

<<<<<<< HEAD
    var transformationMatrix = Matrix<double>.Build.Dense(12, 12);
    transformationMatrix.SetSubMatrix(0, 0, rotationMatrix);
    transformationMatrix.SetSubMatrix(3, 3, rotationMatrix);
    transformationMatrix.SetSubMatrix(6, 6, rotationMatrix);
    transformationMatrix.SetSubMatrix(9, 9, rotationMatrix);
=======
      var rxx = (baseLine.EndNode1.Position[0] - baseLine.EndNode0.Position[0]) / L;
      var rxy = (baseLine.EndNode1.Position[1] - baseLine.EndNode0.Position[1]) / L;
      var rxz = (baseLine.EndNode1.Position[2] - baseLine.EndNode0.Position[2]) / L;
>>>>>>> main

    return transformationMatrix;
  }

  public Matrix<double> GetRotationMatrix()
  {
    if (this.BaseCurve is not Line baseLine)
    {
      throw new NotSupportedException("Curved elements are not supported yet");
    }

    var L = baseLine.Length;

    var rxx = (baseLine.EndNode1.Position[0] - baseLine.EndNode0.Position[0]) / L;
    var rxy = (baseLine.EndNode1.Position[1] - baseLine.EndNode0.Position[1]) / L;
    var rxz = (baseLine.EndNode1.Position[2] - baseLine.EndNode0.Position[2]) / L;

<<<<<<< HEAD
    var cosG = Math.Cos(this.ProfileRotation);
    var sinG = Math.Sin(this.ProfileRotation);

    var sqrtRxx2Rxz2 = Math.Sqrt((rxx * rxx) + (rxz * rxz));

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
      r21 = ((-rxx * rxy * cosG) - (rxz * sinG)) / sqrtRxx2Rxz2;
      r22 = sqrtRxx2Rxz2 * cosG;
      r23 = ((-rxy * rxz * cosG) + (rxx * sinG)) / sqrtRxx2Rxz2;
      r31 = ((rxx * rxy * sinG) - (rxz * cosG)) / sqrtRxx2Rxz2;
      r32 = -sqrtRxx2Rxz2 * sinG;
      r33 = ((rxy * rxz * sinG) + (rxx * cosG)) / sqrtRxx2Rxz2;
    }

    return DenseMatrix.OfArray(new[,] {
      { rxx, rxy, rxz },
      { r21, r22, r23 },
      { r31, r32, r33 },
    });
=======
      return DenseMatrix.OfArray(new[,] {
        { rxx, rxy, rxz },
        { r21, r22, r23 },
        { r31, r32, r33 },
      });
    }
>>>>>>> main
  }
}

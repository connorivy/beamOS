using beamOS.API.Schema.Objects;
using LanguageExt;
using MathNet.Numerics.LinearAlgebra;
using beamOS.Tests.TestObjects;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using beamOS.Tests.Schema.Objects;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Double;
using Speckle.Core.Models;
using Xunit.Abstractions;
using Speckle.Core.Serialisation;
using System.Reflection;

namespace beamOS.Tests.TestObjects.Element1Ds
{

  public class AllElement1DFixtures : TheoryData<Element1DFixture>
  {
    public List<Element1DFixture> Element1DFixtures = new()
    {
      // add standalone element 1D fixtures here
    };
    public AllElement1DFixtures() 
    {
      var allSolvedProblems = new AllSolvedProblems();
      foreach (var solvedProblem in allSolvedProblems.SolvedProblems)
      {
        foreach (var element1DFixture in solvedProblem.Element1DFixtures)
        {
          Add(element1DFixture);
        }
      }

      foreach (var element1DFixture in Element1DFixtures)
      {
        Add(element1DFixture);
      }
    }
  }

  public class Element1DFixture : SerializableFixtureBase<Element1D>, IHasLocalResults, IHasGlobalResults
  {
    public Element1DFixture()
    {
    }
    public Element1DFixture(Element1D element)
    {
      Element = element;
    }

    //public Element1D Element { get; set; }
    public Option<Matrix<double>> ExpectedRotationMatrix { get; set; }
    public Option<Matrix<double>> ExpectedTransformationMatrix { get; set; }
    public Option<Matrix<double>> ExpectedLocalStiffnessMatrix { get; set; }
    public Option<Matrix<double>> ExpectedGlobalStiffnessMatrix { get; set; }
    public Option<Vector<double>> ExpectedLocalFixedEndForces { get; set; }
    public Option<Vector<double>> ExpectedGlobalFixedEndForces { get; set; }
    public Option<Vector<double>> ExpectedLocalEndDisplacements { get; set; }
    public Option<Vector<double>> ExpectedGlobalEndDisplacements { get; set; }
    public Option<Vector<double>> ExpectedLocalEndForces { get; set; }
    public Option<Vector<double>> ExpectedGlobalEndForces { get; set; }

    //public void Deserialize(IXunitSerializationInfo info)
    //{
    //  var deserializerV2 = new BaseObjectDeserializerV2();

    //  var functionMap = new Dictionary<Type, Action<PropertyInfo>>()
    //  {
    //    { 
    //      typeof(Base), 
    //      (prop) => prop.SetValue(this, deserializerV2.Deserialize(info.GetValue<string>(prop.Name)) as Element1D) 
    //    },
    //  };

    //  foreach (PropertyInfo prop in GetType().GetProperties())
    //  {
    //    object? item;
    //    try
    //    {
    //      item = prop.GetValue(this);
    //    }
    //    catch (Exception e)
    //    {
    //      continue;
    //    }

    //    Type type;
    //    if (prop.PropertyType.IsSubclassOf(typeof(Base)))
    //    {
    //      type = typeof(Base);
    //    }
    //    else
    //    {
    //      type = prop.PropertyType;
    //    }

    //    if (!functionMap.ContainsKey(type))
    //      continue;

    //    functionMap[type](prop);


    //    switch (item)
    //    {
    //      case Base:
    //        prop.SetValue(this, deserializerV2.Deserialize(info.GetValue<string>(prop.Name)) as Element1D);
    //        break;
    //      case Option<Matrix<double>>:
    //        var matrixArray = info.GetValue<double[,]>(prop.Name);
    //        if (matrixArray == null)
    //          continue;

    //        Option<Matrix<double>> matrixOption = DenseMatrix.OfArray(matrixArray);
    //        prop.SetValue(this, matrixOption);
    //        break;
    //      case Option<Vector<double>>:
    //        var vectorArray = info.GetValue<double[]>(prop.Name);
    //        if (vectorArray == null)
    //          continue;

    //        var vector = Vector<double>.Build.DenseOfArray(vectorArray);
    //        Option<Vector<double>> vectorOption = vector;
    //        prop.SetValue(this, vectorOption);
    //        break;
    //    }
    //  }
    //  //this.Element = (Element1D)deserializerV2.Deserialize(info.GetValue<string>(nameof(this.Element)));
    //  //ExpectedRotationMatrix = DenseMatrix.OfArray(info.GetValue<double[,]>(nameof(ExpectedRotationMatrix)));
    //  //ExpectedTransformationMatrix = DenseMatrix.OfArray(info.GetValue<double[,]>(nameof(ExpectedTransformationMatrix)));
    //  //ExpectedLocalStiffnessMatrix = DenseMatrix.OfArray(info.GetValue<double[,]>(nameof(ExpectedLocalStiffnessMatrix)));
    //  //ExpectedGlobalStiffnessMatrix = DenseMatrix.OfArray(info.GetValue<double[,]>(nameof(ExpectedGlobalStiffnessMatrix)));
    //  //ExpectedLocalFixedEndForces = Vector<double>.Build.DenseOfArray(info.GetValue<double[]>(nameof(ExpectedLocalFixedEndForces)));
    //  //ExpectedGlobalFixedEndForces = Vector<double>.Build.DenseOfArray(info.GetValue<double[]>(nameof(ExpectedGlobalFixedEndForces)));
    //  //ExpectedLocalEndDisplacements = Vector<double>.Build.DenseOfArray(info.GetValue<double[]>(nameof(ExpectedLocalEndDisplacements)));
    //  //ExpectedGlobalEndDisplacements = Vector<double>.Build.DenseOfArray(info.GetValue<double[]>(nameof(ExpectedGlobalEndDisplacements)));
    //  //ExpectedLocalEndForces = Vector<double>.Build.DenseOfArray(info.GetValue<double[]>(nameof(ExpectedLocalEndForces)));
    //  //ExpectedGlobalEndForces = Vector<double>.Build.DenseOfArray(info.GetValue<double[]>(nameof(ExpectedGlobalEndForces)));
    //  //this.G = info.GetValue<byte>(nameof(this.G));
    //  //this.B = info.GetValue<byte>(nameof(this.B));
    //}

    //public void Serialize(IXunitSerializationInfo info)
    //{
    //  var serializerV2 = new BaseObjectSerializerV2();

    //  foreach (PropertyInfo prop in GetType().GetProperties())
    //  {
    //    object? item;
    //    try
    //    {
    //      item = prop.GetValue(this);
    //    }
    //    catch (Exception e)
    //    {
    //      continue;
    //    }
    //    switch (item)
    //    {
    //      case Element1D element1D:
    //        info.AddValue(prop.Name, serializerV2.Serialize(element1D));
    //        break;
    //      case Option<Matrix<double>> matrix:
    //        matrix.IfSome(
    //          m => info.AddValue(prop.Name, m.AsArray() ?? m.ToArray())
    //        );
    //        break;
    //      case Option<Vector<double>> vector:
    //        vector.IfSome(
    //          v => info.AddValue(prop.Name, v.AsArray() ?? v.ToArray())
    //        );
    //        break;
    //    }
    //  }

    //  //info.AddValue(nameof(this.Element), serializerV2.Serialize(this.Element));
    //  //ExpectedRotationMatrix.IfSome(
    //  //  m => info.AddValue(nameof(ExpectedRotationMatrix), m.AsArray() ?? m.ToArray())
    //  //);
    //  //ExpectedTransformationMatrix.IfSome(m => info.AddValue(nameof(ExpectedTransformationMatrix), m.AsArray()));
    //  //ExpectedLocalStiffnessMatrix.IfSome(m => info.AddValue(nameof(ExpectedLocalStiffnessMatrix), m.AsArray()));
    //  //ExpectedGlobalStiffnessMatrix.IfSome(m => info.AddValue(nameof(ExpectedGlobalStiffnessMatrix), m.AsArray()));
    //  //ExpectedLocalFixedEndForces.IfSome(m => info.AddValue(nameof(ExpectedLocalFixedEndForces), m.AsArray()));
    //  //ExpectedGlobalFixedEndForces.IfSome(m => info.AddValue(nameof(ExpectedGlobalFixedEndForces), m.AsArray()));
    //  //ExpectedLocalEndDisplacements.IfSome(m => info.AddValue(nameof(ExpectedLocalEndDisplacements), m.AsArray()));
    //  //ExpectedLocalEndForces.IfSome(m => info.AddValue(nameof(ExpectedLocalEndForces), m.AsArray()));
    //  //ExpectedGlobalEndForces.IfSome(m => info.AddValue(nameof(ExpectedGlobalEndForces), m.AsArray()));
    //}
  }

  public class AnalyticalModelFixture : IHasGlobalResults
  {
    public AnalyticalModelFixture(AnalyticalModel model)
    {
      AnalyticalModel = model;
    }
    public AnalyticalModel AnalyticalModel { get; set; }
    public Option<Matrix<double>> ExpectedGlobalStiffnessMatrix { get; set; }
    public Option<Vector<double>> ExpectedGlobalFixedEndForces { get; set; }
    public Option<Vector<double>> ExpectedGlobalEndDisplacements { get; set; }
    public Option<Vector<double>> ExpectedGlobalEndForces { get; set; }
  }

  public interface IHasLocalResults
  {
    public Option<Matrix<double>> ExpectedRotationMatrix { get; set; }
    public Option<Matrix<double>> ExpectedTransformationMatrix { get; set; }
    public Option<Matrix<double>> ExpectedLocalStiffnessMatrix { get; set; }
    public Option<Vector<double>> ExpectedLocalFixedEndForces { get; set; }
    public Option<Vector<double>> ExpectedLocalEndDisplacements { get; set; }
    public Option<Vector<double>> ExpectedLocalEndForces { get; set; }
  }

  public interface IHasGlobalResults
  {
    public Option<Matrix<double>> ExpectedGlobalStiffnessMatrix { get; set; }
    public Option<Vector<double>> ExpectedGlobalFixedEndForces { get; set; }
    public Option<Vector<double>> ExpectedGlobalEndDisplacements { get; set; }
    public Option<Vector<double>> ExpectedGlobalEndForces { get; set; }
  }
}

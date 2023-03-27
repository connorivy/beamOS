using beamOS.API.Schema.Objects;
using LanguageExt;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Speckle.Core.Models;
using Speckle.Core.Serialisation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace beamOS.Tests.TestObjects
{
  //public class SerializableFixtureBase<T> : FixtureBase<T>, IXunitSerializable
  //{

  //}
  public class SerializableFixtureBase<T> : IXunitSerializable 
    where T : Base
  {
    [DetachProperty]
    public T Element { get; set; }
    public void Deserialize(IXunitSerializationInfo info)
    {
      var deserializerV2 = new BaseObjectDeserializerV2();

      //var functionMap = new Dictionary<Type, Action<PropertyInfo>>()
      //{
      //  {
      //    typeof(Base),
      //    (prop) => prop.SetValue(this, (T)(object)deserializerV2.Deserialize(info.GetValue<string>(prop.Name)))
      //  },
      //  {
      //    typeof(Option<Matrix<double>>),
      //    (prop) => {
      //      var matrixArray = info.GetValue<double[,]>(prop.Name);
      //      if (matrixArray == null)
      //        return;

      //      Option<Matrix<double>> option = DenseMatrix.OfArray(matrixArray);
      //      prop.SetValue(this, option);
      //    }
      //  },
      //  {
      //    typeof(Option<Vector<double>>),
      //    (prop) => {
      //      var vectorArray = info.GetValue<double[]>(prop.Name);
      //      if (vectorArray == null)
      //        return;

      //      Option<Vector<double>> option = Vector<double>.Build.DenseOfArray(vectorArray);
      //      prop.SetValue(this, option);
      //    }
      //  },
      //};

      foreach (PropertyInfo prop in GetType().GetProperties())
      {
        object? item;
        try
        {
          item = prop.GetValue(this);
        }
        catch (Exception e)
        {
          continue;
        }

        Type type;
        if (prop.PropertyType.IsSubclassOf(typeof(Base)))
        {
          type = typeof(Base);
        }
        else
        {
          type = prop.PropertyType;
        }

        //if (!functionMap.ContainsKey(type))
        //  continue;

        //functionMap[type](prop);
        AssignValueToProp(prop, info);


        //switch (item)
        //{
        //  case Base:
        //    prop.SetValue(this, deserializerV2.Deserialize(info.GetValue<string>(prop.Name)) as Element1D);
        //    break;
        //  case Option<Matrix<double>>:
        //    var matrixArray = info.GetValue<double[,]>(prop.Name);
        //    if (matrixArray == null)
        //      continue;

        //    Option<Matrix<double>> matrixOption = DenseMatrix.OfArray(matrixArray);
        //    prop.SetValue(this, matrixOption);
        //    break;
        //  case Option<Vector<double>>:
        //    var vectorArray = info.GetValue<double[]>(prop.Name);
        //    if (vectorArray == null)
        //      continue;

        //    var vector = Vector<double>.Build.DenseOfArray(vectorArray);
        //    Option<Vector<double>> vectorOption = vector;
        //    prop.SetValue(this, vectorOption);
        //    break;
        //}
      }
      //this.Element = (Element1D)deserializerV2.Deserialize(info.GetValue<string>(nameof(this.Element)));
      //ExpectedRotationMatrix = DenseMatrix.OfArray(info.GetValue<double[,]>(nameof(ExpectedRotationMatrix)));
      //ExpectedTransformationMatrix = DenseMatrix.OfArray(info.GetValue<double[,]>(nameof(ExpectedTransformationMatrix)));
      //ExpectedLocalStiffnessMatrix = DenseMatrix.OfArray(info.GetValue<double[,]>(nameof(ExpectedLocalStiffnessMatrix)));
      //ExpectedGlobalStiffnessMatrix = DenseMatrix.OfArray(info.GetValue<double[,]>(nameof(ExpectedGlobalStiffnessMatrix)));
      //ExpectedLocalFixedEndForces = Vector<double>.Build.DenseOfArray(info.GetValue<double[]>(nameof(ExpectedLocalFixedEndForces)));
      //ExpectedGlobalFixedEndForces = Vector<double>.Build.DenseOfArray(info.GetValue<double[]>(nameof(ExpectedGlobalFixedEndForces)));
      //ExpectedLocalEndDisplacements = Vector<double>.Build.DenseOfArray(info.GetValue<double[]>(nameof(ExpectedLocalEndDisplacements)));
      //ExpectedGlobalEndDisplacements = Vector<double>.Build.DenseOfArray(info.GetValue<double[]>(nameof(ExpectedGlobalEndDisplacements)));
      //ExpectedLocalEndForces = Vector<double>.Build.DenseOfArray(info.GetValue<double[]>(nameof(ExpectedLocalEndForces)));
      //ExpectedGlobalEndForces = Vector<double>.Build.DenseOfArray(info.GetValue<double[]>(nameof(ExpectedGlobalEndForces)));
      //this.G = info.GetValue<byte>(nameof(this.G));
      //this.B = info.GetValue<byte>(nameof(this.B));
    }

    public void AssignValueToProp(PropertyInfo prop, IXunitSerializationInfo info)
    {
      var deserializerV2 = new BaseObjectDeserializerV2();

      var type = prop.PropertyType;
      if (type.IsSubclassOf(typeof(Base)))
      {
        var json = info.GetValue<string>(prop.Name);
        var @base = deserializerV2.Deserialize(json);
        if (@base is T subclass)
          prop.SetValue(this, subclass);
      }
      else if (type == typeof(Option<Matrix<double>>))
      {
        var matrixArray = info.GetValue<double[,]>(prop.Name);
        if (matrixArray == null)
          return;

        Option<Matrix<double>> matrixOption = DenseMatrix.OfArray(matrixArray);
        prop.SetValue(this, matrixOption);
      }
      else if (type == typeof(Option<Vector<double>>))
      {
        var vectorArray = info.GetValue<double[]>(prop.Name);
        if (vectorArray == null)
          return;

        Option<Vector<double>> vectorOption = Vector<double>.Build.DenseOfArray(vectorArray);
        prop.SetValue(this, vectorOption);
      }
    }

    public void Serialize(IXunitSerializationInfo info)
    {
      var serializerV2 = new BaseObjectSerializerV2();

      foreach (PropertyInfo prop in GetType().GetProperties())
      {
        object? item;
        try
        {
          item = prop.GetValue(this);
        }
        catch (Exception e)
        {
          continue;
        }
        switch (item)
        {
          case Element1D element1D:
            info.AddValue(prop.Name, serializerV2.Serialize(element1D));
            break;
          case Option<Matrix<double>> matrix:
            matrix.IfSome(
              m => info.AddValue(prop.Name, m.AsArray() ?? m.ToArray())
            );
            break;
          case Option<Vector<double>> vector:
            vector.IfSome(
              v => info.AddValue(prop.Name, v.AsArray() ?? v.ToArray())
            );
            break;
        }
      }
    }
  }
}

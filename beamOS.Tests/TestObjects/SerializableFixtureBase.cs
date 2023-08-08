namespace beamOS.Tests.TestObjects;

using System.Collections;
using System.Reflection;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Speckle.Core.Models;
using Speckle.Core.Serialisation;
using Xunit.Abstractions;

public class SerializableFixtureBase : IXunitSerializable
{
  public SerializableFixtureBase() { }
  public void Deserialize(IXunitSerializationInfo info)
  {
    foreach (var prop in this.GetType().GetProperties())
    {
      this.AssignValueToProp(prop, info);
    }
  }

  public void AssignValueToProp(PropertyInfo prop, IXunitSerializationInfo info)
  {
    var deserializerV2 = new BaseObjectDeserializerV2();

    var type = prop.PropertyType;
    if (type.IsSubclassOf(typeof(Base)))
    {
      prop.SetValue(this, deserializerV2.Deserialize(info.GetValue<string>(prop.Name)));
    }
    else if (type.IsInterface)
    {
      prop.SetValue(this, deserializerV2.Deserialize(info.GetValue<string>(prop.Name)));
    }
    else if (type == typeof(Matrix<double>))
    {
      var matrixArray = info.GetValue<double[,]>(prop.Name);
      if (matrixArray == null)
      {
        return;
      }

      var matrixOption = DenseMatrix.OfArray(matrixArray);
      prop.SetValue(this, matrixOption);
    }
    else if (type == typeof(Vector<double>))
    {
      var vectorArray = info.GetValue<double[]>(prop.Name);
      if (vectorArray == null)
      {
        return;
      }

      var vectorOption = Vector<double>.Build.DenseOfArray(vectorArray);
      prop.SetValue(this, vectorOption);
    }
    else if (type == typeof(string))
    {
      var stringValue = info.GetValue<string>(prop.Name);
      prop.SetValue(this, stringValue);
    }
    else if (type == typeof(int))
    {
      var stringValue = info.GetValue<int>(prop.Name);
      prop.SetValue(this, stringValue);
    }
    else if (type == typeof(double))
    {
      var stringValue = info.GetValue<double>(prop.Name);
      prop.SetValue(this, stringValue);
    }
    else if (type == typeof(float))
    {
      var stringValue = info.GetValue<float>(prop.Name);
      prop.SetValue(this, stringValue);
    }
    else if (type == typeof(bool))
    {
      var stringValue = info.GetValue<bool>(prop.Name);
      prop.SetValue(this, stringValue);
    }
    else if (type == typeof(double[]))
    {
      var stringValue = info.GetValue<double[]>(prop.Name);
      prop.SetValue(this, stringValue);
    }
  }

  public void Serialize(IXunitSerializationInfo info)
  {
    // if any errors are thrown here, then VS will hang when the user attempts to run tests
    // therefore wrap in try catch if something isn't working
    //try
    //{ 
    var serializerV2 = new BaseObjectSerializerV2();

    foreach (var prop in this.GetType().GetProperties())
    {
      var item = prop.GetValue(this);

      switch (item)
      {
        case Base @base:
          info.AddValue(prop.Name, serializerV2.Serialize(@base));
          continue;
        case Matrix<double> matrix:
          if (matrix != null)
          {
            info.AddValue(prop.Name, matrix.AsArray() ?? matrix.ToArray());
          }
          continue;
        case Vector<double> vector:
          if (vector != null)
          {
            info.AddValue(prop.Name, vector.AsArray() ?? vector.ToArray());
          }
          continue;
        case string:
        case int:
        case double:
        case float:
        case bool:
        case IList:
          info.AddValue(prop.Name, item);
          continue;
        default:
          break;
      }
    }
    //}
    //catch (Exception e )
    //{

    //}
  }
}

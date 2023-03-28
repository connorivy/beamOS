using beamOS.API.Schema.Objects;
using LanguageExt;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Speckle.Core.Models;
using Speckle.Core.Serialisation;
using System.Reflection;
using Xunit.Abstractions;

namespace beamOS.Tests.TestObjects
{
  public class SerializableFixtureBase : IXunitSerializable 
  {
    public SerializableFixtureBase() { }
    public void Deserialize(IXunitSerializationInfo info)
    {
      foreach (PropertyInfo prop in GetType().GetProperties())
      {
        AssignValueToProp(prop, info);
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

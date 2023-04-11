namespace beamOS.Tests.TestObjects;
using LanguageExt;
using LanguageExt.UnsafeValueAccess;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Speckle.Core.Models;
using Speckle.Core.Serialisation;
using System.Reflection;
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
    else if (type == typeof(Option<Matrix<double>>))
    {
      var matrixArray = info.GetValue<double[,]>(prop.Name);
      if (matrixArray == null)
      {
        return;
      }

      Option<Matrix<double>> matrixOption = DenseMatrix.OfArray(matrixArray);
      prop.SetValue(this, matrixOption);
    }
    else if (type == typeof(Option<Vector<double>>))
    {
      var vectorArray = info.GetValue<double[]>(prop.Name);
      if (vectorArray == null)
      {
        return;
      }

      Option<Vector<double>> vectorOption = Vector<double>.Build.DenseOfArray(vectorArray);
      prop.SetValue(this, vectorOption);
    }
    else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Option<>))
    {
      var genericTypeArgument = type.GenericTypeArguments[0];
      var someType = typeof(Some<>);
      var totalSomeType = someType.MakeGenericType(genericTypeArgument);

      var val = info.GetValue<object>(prop.Name);
      var instance = Activator.CreateInstance(totalSomeType, new[] { val });
      var optionInstance = Activator.CreateInstance(type, new[] { instance });
      prop.SetValue(this, optionInstance);
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
      object? item;
      try
      {
        item = prop.GetValue(this);
      }
      catch
      {
        continue;
      }

      if (item == null)
      {
        return;
      }

      switch (item)
      {
        case Base @base:
          info.AddValue(prop.Name, serializerV2.Serialize(@base));
          continue;
        case Option<Matrix<double>> matrix:
          var x = matrix.ValueUnsafe();
          matrix.IfSome(
            m => info.AddValue(prop.Name, m.AsArray() ?? m.ToArray())
          );
          continue;
        case Option<Vector<double>> vector:
          vector.IfSome(
            v => info.AddValue(prop.Name, v.AsArray() ?? v.ToArray())
          );
          continue;
        default:
          break;
      }
      TrySerializeOption(info, prop, item);
    }
    //}
    //catch (Exception e )
    //{

    //}
  }

  private static void TrySerializeOption(IXunitSerializationInfo info, PropertyInfo prop, object item)
  {
    var type = item.GetType();
    if (!type.IsGenericType)
    {
      return;
    }

    var genericType = type.GetGenericTypeDefinition();

    if (genericType != typeof(Option<>))
    {
      return;
    }

    var genericTypeArgument = type.GenericTypeArguments[0];
    var methods = typeof(UnsafeValueAccessExtensions)
      .GetMethods()
      .Where(m => m.Name == "ValueUnsafe");

    MethodInfo? genericMethod = null;
    foreach (var method in methods)
    {
      genericMethod = method.MakeGenericMethod(genericTypeArgument);
      if (genericMethod != null)
      {
        break;
      }
    }

    if (genericMethod == null)
    {
      return;
    }

    var value = genericMethod.Invoke(null, new object[] { item });

    if (value == null)
    {
      return;
    }

    info.AddValue(prop.Name, value);
  }
}

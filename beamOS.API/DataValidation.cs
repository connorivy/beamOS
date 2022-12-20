namespace beamOS.API
{
  public class DataValidation
  {
    public void AssertNumber(object num, string varName)
    {
      var numericTypes = new HashSet<Type>()
      {
        typeof(byte),
        typeof(double),
        typeof(decimal),
        typeof(float),
        typeof(int),
        typeof(long),
        typeof(sbyte),
        typeof(short),
        typeof(uint),
        typeof(ulong),
        typeof(ushort),
      };
      if (!numericTypes.Contains(num.GetType()))
        throw new Exception($"The value for {varName} should be of type int, long, float, or double. Not of type {num.GetType().FullName}");
    }
    public void AssertPositiveNumber(object num, string varName)
    {
      if (num is IConvertible)
      {
        if (Convert.ToDouble(num) >= 0.0)
          return;
        throw new Exception($"Number \"{num}\" is not greater than or equal to 0");
      }
      throw new Exception($"Number \"{num}\" is not of the correct type");
    }
    public void AssertStrictlyPositiveNumber(object num, string varName)
    {
      if (num is IConvertible)
      {
        if (Convert.ToDouble(num) > 0.0)
          return;
        throw new Exception($"Number \"{num}\" is not strictly greater than 0");
      }
      throw new Exception($"Number \"{num}\" is not of the correct type");
    }
  }
}

namespace beamOS.API
{
  public static class DataValidation
  {
    public static void AssertNumber(object num, string varName)
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
    public static void AssertPositiveNumber(object num, string varName)
    {
      if (num is IConvertible)
      {
        if (Convert.ToDouble(num) >= 0.0)
          return;
        throw new Exception($"Number \"{num}\" is not greater than or equal to 0");
      }
      throw new Exception($"Number \"{num}\" is not of the correct type");
    }
    public static void AssertStrictlyPositiveNumber(object num, string varName)
    {
      if (num is IConvertible)
      {
        if (Convert.ToDouble(num) > 0.0)
          return;
        throw new Exception($"Number \"{num}\" is not strictly greater than 0");
      }
      throw new Exception($"Number \"{num}\" is not of the correct type");
    }
    public static void AssertNumbersAlmostEqual(object num1, object num2, string var1Name, string var2Name)
    {
      AssertNumber(num1, var1Name);
      AssertNumber(num2, var2Name);
      var diff = Math.Abs(Convert.ToDouble(num1) - Convert.ToDouble(num2));
      if (diff < .0001)
        return;
      throw new Exception($"Variable {var1Name} has a value of {num1} which is not equal to variable {var2Name} which has a value of {num2}. The difference of these numbers is {diff}");
    }
  }
}

using beamOS.API;
using System;

namespace beamOS.Tests
{
  public sealed class DataValidationTests
  {
    [Theory]
    #region inLineData
    [InlineData("1", "TestAssertNumber1", true)]
    [InlineData((float)5.1, "TestAssertNumber2", false)]
    [InlineData((byte)55, "TestAssertNumber3", false)]
    [InlineData((ushort)123, "TestAssertNumber4", false)]
    [InlineData(new int[] { 5, 5 }, "TestAssertNumber5", true)]
    [InlineData(0, "TestAssertNumber6", false)]
    [InlineData(-5.234, "TestAssertNumber7", false)]
    [InlineData(double.MaxValue, "TestAssertNumber8", false)]
    #endregion
    public void TestAssertNumber(object num, string name, bool exceptionThrown)
    {
      var exception = Record.Exception(() => DataValidation.AssertNumber(num, name));
      if (exceptionThrown)
        Assert.NotNull(exception);
      else
        Assert.Null(exception);
    }
    [Theory]
    #region inLineData
    [InlineData("abc", "TestAssertNumber1", true)]
    [InlineData((float)5.1, "TestAssertNumber2", false)]
    [InlineData((byte)55, "TestAssertNumber3", false)]
    [InlineData((ushort)123, "TestAssertNumber4", false)]
    [InlineData(new int[] { 5, 5 }, "TestAssertNumber5", true)]
    [InlineData(0, "TestAssertNumber6", false)]
    [InlineData(-5.234, "TestAssertNumber7", true)]
    [InlineData(double.MaxValue, "TestAssertNumber8", false)]
    #endregion
    public void TestAssertPositiveNumber(object num, string name, bool exceptionThrown)
    {
      var exception = Record.Exception(() => DataValidation.AssertPositiveNumber(num, name));
      if (exceptionThrown)
        Assert.NotNull(exception);
      else
        Assert.Null(exception);
    }
    [Theory]
    #region inLineData
    [InlineData("abc", "TestAssertNumber1", true)]
    [InlineData((float)5.1, "TestAssertNumber2", false)]
    [InlineData((byte)55, "TestAssertNumber3", false)]
    [InlineData((ushort)123, "TestAssertNumber4", false)]
    [InlineData(new int[] { 5, 5 }, "TestAssertNumber5", true)]
    [InlineData(0, "TestAssertNumber6", true)]
    [InlineData(-5.234, "TestAssertNumber7", true)]
    [InlineData(double.MaxValue, "TestAssertNumber8", false)]
    #endregion
    public void TestAssertStrictlyPositiveNumber(object num, string name, bool exceptionThrown)
    {
      var exception = Record.Exception(() => DataValidation.AssertStrictlyPositiveNumber(num, name));
      if (exceptionThrown)
        Assert.NotNull(exception);
      else
        Assert.Null(exception);
    }
  }
}

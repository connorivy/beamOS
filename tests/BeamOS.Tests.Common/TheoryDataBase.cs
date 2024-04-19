using Xunit;

namespace BeamOS.Tests.Common;

public class TheoryDataBase<TData> : TheoryData<TData>
{
    public TheoryDataBase(IEnumerable<TData> data)
    {
        foreach (var item in data)
        {
            this.Add(item);
        }
    }

    protected TheoryDataBase() { }

    public IEnumerable<TData> GetItems()
    {
        foreach (var item in this)
        {
            yield return (TData)item[0];
        }
    }
}

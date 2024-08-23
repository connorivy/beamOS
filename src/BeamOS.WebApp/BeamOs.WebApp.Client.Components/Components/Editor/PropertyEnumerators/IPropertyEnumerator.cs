using System.Diagnostics.Contracts;
using BeamOs.Contracts.PhysicalModel.Common;
using BeamOs.Contracts.PhysicalModel.Node;

namespace BeamOs.WebApp.Client.Components.Components.Editor.PropertyEnumerators;

public readonly struct ContractProp
{
    public required string Name { get; init; }
    public required Type PropertyType { get; init; }
    public required object Value { get; init; }
    public bool ShouldInline { get; init; }
}

public interface IPropertyEnumerator<TContract> : IPropertyEnumerator
    where TContract : BeamOsContractBase
{
    //IEnumerable<ContractProp> GetProps(TContract contract);
    IEnumerable<(string, string)> GetProps(TContract contract, string? prefix = null);
}

public interface IPropertyEnumerator
{
    IEnumerable<(string, string)> GetProps(object contract, string? prefix = null);
}

//public class NodePropertyEnumerator : IPropertyEnumerator<NodeResponse>
//{
//    public IEnumerable<ContractProp> GetProps(NodeResponse contract)
//    {
//        yield return new()
//        {
//            Name = "Id",
//            PropertyType = typeof(string),
//            Value = contract.Id
//        };
//        yield return new()
//        {
//            Name = "Id",
//            PropertyType = typeof(string),
//            Value = contract.Id
//        };
//        yield return new()
//        {
//            Name = "Id",
//            PropertyType = typeof(string),
//            Value = contract.Id
//        };
//    }

//    public IEnumerable<(string, string)> GetProps2(NodeResponse contract)
//    {
//        yield return ("Id", contract.Id);
//    }
//}

public abstract class PropertyEnumeratorBase { }

public class PointPropertyEnumerator : IPropertyEnumerator<Point>
{
    //public IEnumerable<ContractProp> GetProps(Point contract, string? prefix = null)
    //{
    //    yield return new()
    //    {
    //        Name = GetPropertyName(prefix, nameof(contract.XCoordinate)),
    //        PropertyType = typeof(double),
    //        Value = contract.XCoordinate
    //    };
    //    yield return new()
    //    {
    //        Name = GetPropertyName(prefix, nameof(contract.YCoordinate)),
    //        PropertyType = typeof(double),
    //        Value = contract.YCoordinate
    //    };
    //    yield return new()
    //    {
    //        Name = GetPropertyName(prefix, nameof(contract.ZCoordinate)),
    //        PropertyType = typeof(double),
    //        Value = contract.ZCoordinate
    //    };
    //    yield return new()
    //    {
    //        Name = GetPropertyName(prefix, nameof(contract.LengthUnit)),
    //        PropertyType = typeof(string),
    //        Value = contract.LengthUnit
    //    };
    //}

    public IEnumerable<(string, string)> GetProps(Point contract, string? prefix = null)
    {
        yield return (
            GetPropertyName(prefix, nameof(contract.XCoordinate)),
            contract.XCoordinate.ToString()
        );
        yield return (
            GetPropertyName(prefix, nameof(contract.YCoordinate)),
            contract.YCoordinate.ToString()
        );
        yield return (
            GetPropertyName(prefix, nameof(contract.ZCoordinate)),
            contract.ZCoordinate.ToString()
        );
        yield return (
            GetPropertyName(prefix, nameof(contract.LengthUnit)),
            contract.LengthUnit.ToString()
        );
    }

    private static string GetPropertyName(string? prefix, string propName)
    {
        return prefix != null ? $"{prefix}.{propName}" : propName;
    }

    public IEnumerable<(string, string)> GetProps2(Point contract)
    {
        yield return ("Id", "");
    }

    public IEnumerable<(string, string)> GetProps(object contract, string? prefix = null) =>
        this.GetProps((Point)contract, prefix);
}

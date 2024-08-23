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

public interface IPropertyEnumerator<TContract>
    where TContract : BeamOsContractBase
{
    IEnumerable<ContractProp> GetProps(TContract contract);
    IEnumerable<(string, string)> GetProps2(TContract contract);
}

public class NodePropertyEnumerator : IPropertyEnumerator<NodeResponse>
{
    public IEnumerable<ContractProp> GetProps(NodeResponse contract)
    {
        yield return new()
        {
            Name = "Id",
            PropertyType = typeof(string),
            Value = contract.Id
        };
        yield return new()
        {
            Name = "Id",
            PropertyType = typeof(string),
            Value = contract.Id
        };
        yield return new()
        {
            Name = "Id",
            PropertyType = typeof(string),
            Value = contract.Id
        };
    }

    public IEnumerable<(string, string)> GetProps2(NodeResponse contract)
    {
        yield return ("Id", contract.Id);
    }
}

public class PointPropertyEnumerator
{
    public IEnumerable<ContractProp> GetProps(PointResponse contract, string? prefix = null)
    {
        yield return new()
        {
            Name = $"{prefix}.{nameof(contract.XCoordinate)}",
            PropertyType = typeof(double),
            Value = contract.XCoordinate
        };
        yield return new()
        {
            Name = $"{prefix}.{nameof(contract.YCoordinate)}",
            PropertyType = typeof(double),
            Value = contract.YCoordinate
        };
        yield return new()
        {
            Name = $"{prefix}.{nameof(contract.ZCoordinate)}",
            PropertyType = typeof(double),
            Value = contract.ZCoordinate
        };
        yield return new()
        {
            Name = $"{prefix}.{nameof(contract.ZCoordinate)}",
            PropertyType = typeof(double),
            Value = contract.ZCoordinate
        };
    }

    public IEnumerable<(string, string)> GetProps2(PointResponse contract)
    {
        yield return ("Id", "");
    }
}

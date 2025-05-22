using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Sdk;
using Objects.BuiltElements.Revit;

namespace BeamOs.SpeckleConnector;

public class SpeckleReceiveOperationContext
{
    public BeamOsModelProposalBuilder ProposalBuilder { get; } = new();
    private readonly Dictionary<Type, Dictionary<string, int>> cache = [];

    public int? GetConvertedId(Type type, string identifier)
    {
        if (
            this.cache.TryGetValue(type, out var typeCache)
            && typeCache.TryGetValue(identifier, out int id)
        )
        {
            return id;
        }
        return null;
    }

    public void SaveConvertedId(Type type, string identifier, int id)
    {
        if (!this.cache.TryGetValue(type, out var typeCache))
        {
            typeCache = [];
            this.cache[type] = typeCache;
        }
        typeCache[identifier] = id;
    }

    public void AddIssue(
        BeamOsObjectType type,
        string identifier,
        int id,
        ProposalIssueCode code,
        ProposalIssueSeverity? severity = null,
        string? message = null
    )
    {
        this.ProposalBuilder.ProposalIssues.Add(
            new()
            {
                ProposedId = ProposedID.Proposed(id),
                ObjectType = type,
                Message = message ?? "The object with identifier " + identifier + " was not found.",
                Severity = severity ?? ProposalIssueSeverity.Critical,
                Code = code,
            }
        );
    }

    public int GetNextSectionProfileId()
    {
        return this.ProposalBuilder.CreateSectionProfileFromLibraryProposals.Count
            + this.ProposalBuilder.CreateSectionProfileProposals.Count
            + 1;
    }

    public static LengthUnitContract SpeckleUnitsToUnitsNet(string speckleUnit)
    {
        return speckleUnit switch
        {
            "mm" or "millimeter" => LengthUnitContract.Millimeter,
            "cm" or "centimeter" => LengthUnitContract.Centimeter,
            "m" or "meter" => LengthUnitContract.Meter,
            "ft" or "feet" or "foot" => LengthUnitContract.Foot,
            "in" or "inch" => LengthUnitContract.Inch,
            _ => throw new NotSupportedException("Unsupported unit: " + speckleUnit),
        };
    }
}

public class RevitBeamConverter(
    SpeckleReceiveOperationContext context,
    RevitNodeConverter revitNodeConverter,
    RevitSectionProfileConverter revitSectionProfileConverter,
    RevitMaterialConverter revitMaterialConverter
) : ToProposalConverter<RevitBeam>(context), ITopLevelProposalConverter<RevitBeam>
{
    protected override int AddToProposalBuilder(RevitBeam item)
    {
        int id = this.Context.ProposalBuilder.CreateElement1dProposals.Count + 1;
        if (item.baseLine is not Objects.Geometry.Line baseline)
        {
            this.Context.AddIssue(
                BeamOsObjectType.Element1d,
                item.id,
                id,
                ProposalIssueCode.CouldNotCreateProposedObject,
                ProposalIssueSeverity.Error,
                "Unsupported baseLine type: " + item.baseLine.GetType()
            );
            return id;
        }

        var startNodeId = revitNodeConverter.ConvertAndReturnId(baseline.start);
        var endNodeId = revitNodeConverter.ConvertAndReturnId(baseline.end);

        this.Context.ProposalBuilder.CreateElement1dProposals.Add(
            new()
            {
                Id = id,
                StartNodeId = ProposedID.Proposed(startNodeId),
                EndNodeId = ProposedID.Proposed(endNodeId),
                SectionProfileId = ProposedID.Proposed(
                    revitSectionProfileConverter.ConvertAndReturnId(item.type)
                ),
                MaterialId = ProposedID.Proposed(
                    revitMaterialConverter.ConvertAndReturnId(item.type)
                ),
            }
        );

        return id;
    }

    protected override string GetIdentifier(RevitBeam item) => item.id;
}

public class RevitColumnConverter(
    SpeckleReceiveOperationContext context,
    RevitNodeConverter revitNodeConverter,
    RevitSectionProfileConverter revitSectionProfileConverter,
    RevitMaterialConverter revitMaterialConverter
) : ToProposalConverter<RevitColumn>(context), ITopLevelProposalConverter<RevitColumn>
{
    protected override int AddToProposalBuilder(RevitColumn item)
    {
        int id = this.Context.ProposalBuilder.CreateElement1dProposals.Count + 1;
        if (item.baseLine is not Objects.Geometry.Line baseline)
        {
            this.Context.AddIssue(
                BeamOsObjectType.Element1d,
                item.id,
                id,
                ProposalIssueCode.CouldNotCreateProposedObject,
                ProposalIssueSeverity.Error,
                "Unsupported baseLine type: " + item.baseLine.GetType()
            );
            return id;
        }

        var startNodeId = revitNodeConverter.ConvertAndReturnId(baseline.start);
        var endNodeId = revitNodeConverter.ConvertAndReturnId(baseline.end);

        this.Context.ProposalBuilder.CreateElement1dProposals.Add(
            new()
            {
                Id = id,
                StartNodeId = ProposedID.Proposed(startNodeId),
                EndNodeId = ProposedID.Proposed(endNodeId),
                SectionProfileId = ProposedID.Proposed(
                    revitSectionProfileConverter.ConvertAndReturnId(item.type)
                ),
                MaterialId = ProposedID.Proposed(
                    revitMaterialConverter.ConvertAndReturnId(item.type)
                ),
            }
        );

        return id;
    }

    protected override string GetIdentifier(RevitColumn item) => item.id;
}

public class RevitNodeConverter(SpeckleReceiveOperationContext context)
    : ToProposalConverter<Objects.Geometry.Point>(context)
{
    protected override int AddToProposalBuilder(Objects.Geometry.Point item)
    {
        var id = this.Context.ProposalBuilder.CreateNodeProposals.Count + 1;
        var lengthUnit = SpeckleReceiveOperationContext.SpeckleUnitsToUnitsNet(item.units);
        this.Context.ProposalBuilder.CreateNodeProposals.Add(
            new()
            {
                Id = id,
                LocationPoint = new(item.x, item.y, item.z, lengthUnit),
                Restraint = Restraint.Free,
            }
        );
        this.Context.AddIssue(
            BeamOsObjectType.Node,
            item.id,
            id,
            ProposalIssueCode.SomeInfoInferred,
            ProposalIssueSeverity.Warning,
            "Node restraints were inferred"
        );
        return id;
    }

    protected override string GetIdentifier(Objects.Geometry.Point item) => item.id;
}

public class RevitSectionProfileConverter(SpeckleReceiveOperationContext context)
    : ToProposalConverter<SectionProfileName>(context)
{
    protected override int AddToProposalBuilder(SectionProfileName item)
    {
        var id = this.Context.GetNextSectionProfileId();

        if (StructuralShapes.Lib.AISC.v16_0.WShapes.GetShapeByName(item) is not null)
        {
            this.Context.ProposalBuilder.CreateSectionProfileFromLibraryProposals.Add(
                new()
                {
                    Id = id,
                    Name = item,
                    Library = StructuralCode.AISC_360_16,
                }
            );
        }
        else
        {
            this.Context.ProposalBuilder.CreateSectionProfileProposals.Add(
                new()
                {
                    Id = id,
                    Name = item + "_placeholder",
                    Area = 0,
                    StrongAxisMomentOfInertia = 0,
                    WeakAxisMomentOfInertia = 0,
                    PolarMomentOfInertia = 0,
                    StrongAxisPlasticSectionModulus = 0,
                    WeakAxisPlasticSectionModulus = 0,
                    StrongAxisShearArea = 0,
                    WeakAxisShearArea = 0,
                    LengthUnit = LengthUnitContract.Meter,
                }
            );

            this.Context.AddIssue(
                BeamOsObjectType.SectionProfile,
                item,
                id,
                ProposalIssueCode.SwappedInPlaceholder,
                ProposalIssueSeverity.Error,
                $"missing section profile information for type name {item}"
            );
        }

        return id;
    }

    protected override string GetIdentifier(SectionProfileName item) => item;
}

public readonly record struct SectionProfileName(string Name)
{
    public static implicit operator SectionProfileName(string name) => new(name);

    public static implicit operator string(SectionProfileName sectionProfile) =>
        sectionProfile.Name;
}

public readonly record struct MaterialName(string Name)
{
    public static implicit operator MaterialName(string name) => new(name);

    public static implicit operator string(MaterialName sectionProfile) => sectionProfile.Name;
}

public class RevitMaterialConverter(SpeckleReceiveOperationContext context)
    : ToProposalConverter<MaterialName>(context)
{
    protected override int AddToProposalBuilder(MaterialName item)
    {
        var id = this.Context.ProposalBuilder.CreateMaterialProposals.Count + 1;
        this.Context.ProposalBuilder.CreateMaterialProposals.Add(
            new()
            {
                Id = id,
                ModulusOfElasticity = 0,
                ModulusOfRigidity = 0,
                PressureUnit = PressureUnitContract.NewtonPerSquareMeter,
            }
        );
        this.Context.AddIssue(
            BeamOsObjectType.Material,
            item,
            id,
            ProposalIssueCode.SwappedInPlaceholder,
            ProposalIssueSeverity.Error,
            "missing section profile information"
        );
        return id;
    }

    protected override string GetIdentifier(MaterialName item) => item;
}

public interface ITopLevelProposalConverter<T> : ITopLevelProposalConverter
{
    public int ConvertAndReturnId(T item);
}

public interface ITopLevelProposalConverter
{
    public int ConvertAndReturnId(object item);
}

public abstract class ToProposalConverter<T>(SpeckleReceiveOperationContext context)
{
    protected SpeckleReceiveOperationContext Context => context;
    protected abstract int AddToProposalBuilder(T item);

    protected abstract string GetIdentifier(T item);

    public int ConvertAndReturnId(T item)
    {
        var identifier = this.GetIdentifier(item);

        if (context.GetConvertedId(typeof(T), identifier) is int id)
        {
            return id;
        }
        var newId = this.AddToProposalBuilder(item);
        context.SaveConvertedId(typeof(T), identifier, newId);
        return newId;
    }

    public int ConvertAndReturnId(object item)
    {
        if (item is not T typedItem)
        {
            throw new InvalidOperationException(
                "Item is not of the expected type: " + typeof(T).FullName
            );
        }
        return this.ConvertAndReturnId(typedItem);
    }
}

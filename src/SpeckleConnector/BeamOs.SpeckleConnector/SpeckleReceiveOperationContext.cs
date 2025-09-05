using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Sdk;
using Speckle.Objects.Data;
using Speckle.Sdk.Models.Collections;

namespace BeamOs.SpeckleConnector;

public class SpeckleReceiveOperationContext
{
    public BeamOsModelProposalBuilder ProposalBuilder { get; } = new();
    private readonly Dictionary<BeamOsObjectType, Dictionary<string, int>> cache = [];
    private readonly Dictionary<string, Length> levelCache = [];

    public void AddLevel(string name, Length level)
    {
        this.levelCache[name] = level;
    }

    public Length? GetLevelElevation(string name)
    {
        if (name == "Top of Parapet")
        {
            return new Length(6141, LengthUnit.Millimeter);
        }
        else if (name == "Level 2")
        {
            return new Length(2900, LengthUnit.Millimeter);
        }
        else if (name == "Ref Beam system")
        {
            return new Length(2398, LengthUnit.Millimeter);
        }
        if (this.levelCache.TryGetValue(name, out var level))
        {
            return level;
        }
        return null;
    }

    public int? GetConvertedId(BeamOsObjectType type, string identifier)
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

    public void SaveConvertedId(BeamOsObjectType type, string identifier, int id)
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

    public static LengthUnitContract SpeckleUnitsToContract(string speckleUnit)
    {
        var unitsNetUnit = SpeckleUnitsToUnitsNet(speckleUnit);
        return unitsNetUnit.MapToContract();
    }

    public static LengthUnit SpeckleUnitsToUnitsNet(string speckleUnit)
    {
        return speckleUnit.ToLowerInvariant() switch
        {
            "mm" or "millimeter" or "millimeters" => LengthUnit.Millimeter,
            "cm" or "centimeter" or "centimeters" => LengthUnit.Centimeter,
            "m" or "meter" or "meters" => LengthUnit.Meter,
            "ft" or "foot" or "feet" => LengthUnit.Foot,
            "in" or "inch" or "inches" => LengthUnit.Inch,
            _ => throw new NotSupportedException("Unsupported unit: " + speckleUnit),
        };
    }
}

public class RevitBeamConverter(
    SpeckleReceiveOperationContext context,
    RevitNodeConverter revitNodeConverter,
    RevitSectionProfileConverter revitSectionProfileConverter,
    RevitMaterialConverter revitMaterialConverter
) : ToProposalConverter<RevitObject>(context)
{
    protected override BeamOsObjectType BeamOsObjectType => BeamOsObjectType.Element1d;

    protected override int AddToProposalBuilder(RevitObject item)
    {
        int id = this.Context.ProposalBuilder.CreateElement1dProposals.Count + 1;
        if (item.location is not Speckle.Objects.Geometry.Line baseline)
        {
            this.Context.AddIssue(
                BeamOsObjectType.Element1d,
                item.id,
                id,
                ProposalIssueCode.CouldNotCreateProposedObject,
                ProposalIssueSeverity.Error,
                "Unsupported baseLine type: " + item.location?.GetType()
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

    protected override string GetIdentifier(RevitObject item) =>
        item.id
        ?? throw new ArgumentNullException(nameof(item.id), "RevitObject id cannot be null");
}

public class RevitColumnConverter(
    SpeckleReceiveOperationContext context,
    RevitNodeConverter revitNodeConverter,
    RevitSectionProfileConverter revitSectionProfileConverter,
    RevitMaterialConverter revitMaterialConverter
) : ToProposalConverter<RevitObject>(context)
{
    protected override BeamOsObjectType BeamOsObjectType => BeamOsObjectType.Element1d;

    protected override int AddToProposalBuilder(RevitObject item)
    {
        int id = this.Context.ProposalBuilder.CreateElement1dProposals.Count + 1;
        if (item.location is not Speckle.Objects.Geometry.Point point)
        {
            this.Context.AddIssue(
                BeamOsObjectType.Element1d,
                item.id,
                id,
                ProposalIssueCode.CouldNotCreateProposedObject,
                ProposalIssueSeverity.Error,
                "Unsupported location type: " + item.location?.GetType()
            );
            return id;
        }

        var constraints =
            (IReadOnlyDictionary<string, object>)
                (
                    (IReadOnlyDictionary<string, object>)
                        ((IReadOnlyDictionary<string, object>)item.properties["Parameters"])[
                            "Instance Parameters"
                        ]
                )["Constraints"];
        var levelName = (string)
            ((IReadOnlyDictionary<string, object>)constraints["Top Level"])["value"];
        var topOffsetObject = ((IReadOnlyDictionary<string, object>)constraints["Top Offset"])[
            "value"
        ];
        var topOffset = Convert.ToDouble(topOffsetObject);
        var offsetUnit = (string)
            ((IReadOnlyDictionary<string, object>)constraints["Top Offset"])["units"];
        var strongOffsetUnits = SpeckleReceiveOperationContext.SpeckleUnitsToUnitsNet(offsetUnit);
        var pointUnits = SpeckleReceiveOperationContext.SpeckleUnitsToUnitsNet(point.units);

        var levelElevation =
            this.Context.GetLevelElevation(levelName)
            ?? throw new ArgumentException($"Level '{levelName}' not found in the context.");

        var topPoint = new Speckle.Objects.Geometry.Point(
            point.x,
            point.y,
            (levelElevation + new Length(topOffset, strongOffsetUnits)).As(pointUnits),
            point.units
        )
        {
            id = item.id + "_topPoint",
        };

        var startNodeId = revitNodeConverter.ConvertAndReturnId(point);
        var endNodeId = revitNodeConverter.ConvertAndReturnId(topPoint);

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

    protected override string GetIdentifier(RevitObject item) =>
        item.id
        ?? throw new ArgumentNullException(nameof(item.id), "RevitObject id cannot be null");
}

public class RevitNodeConverter(SpeckleReceiveOperationContext context)
    : ToProposalConverter<Speckle.Objects.Geometry.Point>(context)
{
    protected override BeamOsObjectType BeamOsObjectType => BeamOsObjectType.Node;

    protected override int AddToProposalBuilder(Speckle.Objects.Geometry.Point item)
    {
        var id = this.Context.ProposalBuilder.CreateNodeProposals.Count + 1;
        var lengthUnit = SpeckleReceiveOperationContext.SpeckleUnitsToContract(item.units);
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

    protected override string GetIdentifier(Speckle.Objects.Geometry.Point item) => item.id;
}

public class RevitSectionProfileConverter(SpeckleReceiveOperationContext context)
    : ToProposalConverter<SectionProfileName>(context)
{
    protected override BeamOsObjectType BeamOsObjectType => BeamOsObjectType.SectionProfile;

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
    protected override BeamOsObjectType BeamOsObjectType => BeamOsObjectType.Material;

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

public class CollectionConverter(SpeckleReceiveOperationContext context)
    : ToProposalConverter<Collection>(context)
{
    protected override BeamOsObjectType BeamOsObjectType => BeamOsObjectType.Other;

    protected override int AddToProposalBuilder(Collection item)
    {
        if (item["properties"] is not IReadOnlyDictionary<string, object> properties)
        {
            return 0;
        }
        if (
            properties["elevation"] is not double elevation
            || properties["units"] is not string units
        )
        {
            return 0;
        }

        var strongUnits = SpeckleReceiveOperationContext.SpeckleUnitsToUnitsNet(units);
        this.Context.AddLevel(item.name, new Length(elevation, strongUnits));

        return 0;
    }

    protected override string GetIdentifier(Collection item) => "Collection_" + item.name;
}

public abstract class ToProposalConverter
{
    public abstract int ConvertAndReturnId(object item);
}

public abstract class ToProposalConverter<TObject>(SpeckleReceiveOperationContext context)
    : ToProposalConverter
{
    protected SpeckleReceiveOperationContext Context => context;
    protected abstract int AddToProposalBuilder(TObject item);

    protected abstract string GetIdentifier(TObject item);

    protected abstract BeamOsObjectType BeamOsObjectType { get; }

    public int ConvertAndReturnId(TObject item)
    {
        var identifier = this.GetIdentifier(item);

        if (context.GetConvertedId(this.BeamOsObjectType, identifier) is int id)
        {
            return id;
        }
        var newId = this.AddToProposalBuilder(item);
        context.SaveConvertedId(this.BeamOsObjectType, identifier, newId);
        return newId;
    }

    public override int ConvertAndReturnId(object item)
    {
        if (item is not TObject typedItem)
        {
            throw new ArgumentException(
                $"Expected item of type {typeof(TObject).Name}, but got {item.GetType().Name}."
            );
        }
        return this.ConvertAndReturnId(typedItem);
    }
}

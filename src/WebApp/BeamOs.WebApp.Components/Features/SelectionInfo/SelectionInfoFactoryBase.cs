using System.Reflection;
using System.Text.Json.Nodes;

namespace BeamOs.WebApp.Components.Features.SelectionInfo;

public abstract class SelectionInfoFactoryBase
{
    public abstract ISelectionInfo CreateSimple(
        object? obj,
        Type fieldType,
        string fieldName,
        bool isRequired = false
    );

    public abstract ISelectionInfo CreateNested(
        object? obj,
        PropertyInfo propInfo,
        int flattenDepth = 0,
        bool isRequired = false
    );

    public ISelectionInfo Create(
        object? obj,
        Type objectType,
        string name,
        bool isRequired = false,
        int flattenDepth = 0,
        List<ISelectionInfo>? additionalSelectionInfo = null
    )
    {
        string properName;
        if (char.IsLower(name[0]))
        {
            properName = char.ToUpperInvariant(name[0]) + name[1..];
        }
        else
        {
            properName = name;
        }

        var nonNullType = Nullable.GetUnderlyingType(objectType) ?? objectType;
        if (SelectionInfoSingleItemComponent.IsSimpleType(nonNullType))
        {
            return this.CreateSimple(obj, nonNullType, properName, isRequired);
        }
        else
        {
            List<ISelectionInfo> selectionInfos = [];
            foreach (
                var propInfo in SelectionInfoSingleItemComponent
                    .GetPublicInstanceProps(nonNullType)
                    ?.Where(p => p.GetSetMethod() is not null && p.Name != "Metadata") ?? []
            )
            {
                selectionInfos.Add(this.CreateNested(obj, propInfo, flattenDepth - 1, isRequired));
            }

            selectionInfos.AddRange(additionalSelectionInfo ?? []);

            return new ComplexFieldSelectionInfo()
            {
                FieldName = properName,
                FieldType = nonNullType,
                SelectionInfos = selectionInfos,
                Flatten = flattenDepth > 0,
                IsRequired = isRequired
            };
        }
    }
}

public class EditableSelectionInfoFactory(Guid modelId) : SelectionInfoFactoryBase
{
    private int numFields;

    public override ISelectionInfo CreateSimple(
        object? obj,
        Type fieldType,
        string fieldName,
        bool isRequired = false
    )
    {
        if (fieldName == "ModelId")
        {
            return new SimpleSelectionInfo()
            {
                FieldName = fieldName,
                FieldType = fieldType,
                Value = modelId
            };
        }
        return new SimpleFieldSelectionInfo()
        {
            FieldName = fieldName,
            FieldType = fieldType,
            FieldNum = this.numFields++,
            IsRequired = isRequired,
            Value = obj
        };
    }

    public override ISelectionInfo CreateNested(
        object? obj,
        PropertyInfo propInfo,
        int flattenDepth = 0,
        bool isRequired = false
    )
    {
        var isPropRequired =
            propInfo.GetCustomAttribute<System.Runtime.CompilerServices.RequiredMemberAttribute>()
                is not null;

        if (obj is null)
        {
            return this.Create(
                null,
                propInfo.PropertyType,
                propInfo.Name,
                isRequired && isPropRequired,
                flattenDepth - 1
            );
        }
        else
        {
            var value = propInfo.GetValue(obj);
            return this.Create(
                value,
                propInfo.PropertyType,
                propInfo.Name,
                isRequired && isPropRequired,
                flattenDepth - 1
            );
        }
    }
}

public class SelectionInfoFactory : SelectionInfoFactoryBase
{
    public override ISelectionInfo CreateSimple(
        object? obj,
        Type fieldType,
        string fieldName,
        bool isRequired = false
    )
    {
        return new SimpleSelectionInfo()
        {
            FieldName = fieldName,
            FieldType = fieldType,
            Value = obj
        };
    }

    public override ISelectionInfo CreateNested(
        object? obj,
        PropertyInfo propInfo,
        int flattenDepth = 0,
        bool isRequired = false
    )
    {
        var value = propInfo.GetValue(obj);
        return this.Create(
            value,
            propInfo.PropertyType,
            propInfo.Name,
            isRequired,
            flattenDepth - 1
        );
    }
}

public interface ISelectionInfo
{
    public string FieldName { get; }
    public Type FieldType { get; init; }
}

public interface ISimpleSelectionInfo : ISelectionInfo
{
    public object? Value { get; set; }

    public int? ValueAsInt
    {
        get => (int?)this.Value;
        set => this.Value = value;
    }

    public double? ValueAsDouble
    {
        get => (double?)this.Value;
        set => this.Value = value;
    }

    public string? ValueAsString
    {
        get => (string?)this.Value;
        set => this.Value = value;
    }

    public bool? ValueAsBool
    {
        get => (bool?)this.Value;
        set => this.Value = value;
    }
}

public interface IEditableSelectionInfo : ISelectionInfo
{
    public int FieldNum { get; set; }
    public bool IsRequired { get; init; }
    public void SetValue(object? value);
}

public interface IComplexSelectionInfo : ISelectionInfo
{
    public List<ISelectionInfo> SelectionInfos { get; }

    public bool Flatten { get; }
}

public record SimpleSelectionInfo : ISimpleSelectionInfo
{
    public required string FieldName { get; init; }
    public required Type FieldType { get; init; }
    public object? Value { get; set; }
}

public record SimpleFieldSelectionInfo : IEditableSelectionInfo, ISimpleSelectionInfo
{
    public required string FieldName { get; init; }
    public required Type FieldType { get; init; }
    public required int FieldNum { get; set; }
    public bool IsRequired { get; init; }
    public object? Value { get; set; }

    public void SetValue(object? value) => this.Value = value;
}

public readonly struct ComplexFieldSelectionInfo : IComplexSelectionInfo
{
    public required string FieldName { get; init; }
    public required Type FieldType { get; init; }
    public required List<ISelectionInfo> SelectionInfos { get; init; }
    public bool Flatten { get; init; }
    public bool IsRequired { get; init; }

    public JsonObject? ToJsonObject()
    {
        if (!HasAnyValues(this))
        {
            return null;
        }

        JsonObject jsonObject = new();

        foreach (var selectionInfo in this.SelectionInfos)
        {
            if (selectionInfo is ISimpleSelectionInfo simpleSelectionInfo)
            {
                bool isRequired = (selectionInfo as IEditableSelectionInfo)?.IsRequired ?? false;

                AddObjectToJsonObject(
                    jsonObject,
                    selectionInfo.FieldName,
                    simpleSelectionInfo.Value,
                    simpleSelectionInfo.FieldType,
                    isRequired
                );
            }
            else if (selectionInfo is ComplexFieldSelectionInfo complex)
            {
                AddObjectToJsonObject(
                    jsonObject,
                    selectionInfo.FieldName,
                    complex.ToJsonObject(),
                    complex.FieldType,
                    complex.IsRequired
                );
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        return jsonObject;
    }

    private static bool HasAnyValues(IComplexSelectionInfo complexSelectionInfo)
    {
        foreach (var selectionInfo in complexSelectionInfo.SelectionInfos)
        {
            if (
                selectionInfo is ISimpleSelectionInfo simpleSelectionInfo
                && simpleSelectionInfo.Value is not null
            )
            {
                return true;
            }
            else if (
                selectionInfo is IComplexSelectionInfo nestedComplexSelection
                && HasAnyValues(nestedComplexSelection)
            )
            {
                return true;
            }
        }
        return false;
    }

    static void AddObjectToJsonObject(
        JsonObject jsonObject,
        string key,
        object? value,
        Type fieldType,
        bool isRequired
    )
    {
        switch (value)
        {
            case null:
                if (isRequired)
                {
                    jsonObject[key] = JsonValue.Create(
                        fieldType.IsValueType ? Activator.CreateInstance(fieldType) : null
                    );
                }
                else
                {
                    jsonObject[key] = null;
                }
                break;
            case int intValue:
                jsonObject[key] = JsonValue.Create(intValue);
                break;
            case bool boolValue:
                jsonObject[key] = JsonValue.Create(boolValue);
                break;
            case string stringValue:
                jsonObject[key] = JsonValue.Create(stringValue);
                break;
            case double doubleValue:
                jsonObject[key] = JsonValue.Create(doubleValue);
                break;
            case float floatValue:
                jsonObject[key] = JsonValue.Create(floatValue);
                break;
            case JsonObject jsonObjectValue:
                jsonObject[key] = jsonObjectValue;
                break;
            default:
                jsonObject[key] = JsonValue.Create(value.ToString());
                break;
        }
    }

    private static object? GetDefaultValue(Type t)
    {
        if (t.IsValueType)
        {
            return Activator.CreateInstance(t);
        }

        return null;
    }
}

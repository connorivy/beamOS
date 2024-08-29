using System.Reflection;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using BeamOs.WebApp.Client.Components.Components.Editor;
using Fluxor;

namespace BeamOs.WebApp.Client.Components.Features.StructuralApiClient;

public readonly record struct ApiClientMethodSelected(MethodInfo? MethodInfo);

public static class ApiClientMethodSelectedActionReducer
{
    [ReducerMethod]
    public static StructuralApiClientState ReduceApiClientMethodSelectedAction(
        StructuralApiClientState state,
        ApiClientMethodSelected action
    )
    {
        if (action.MethodInfo is null)
        {
            return state with
            {
                SelectedMethod = null,
                ParameterValues = null,
                CurrentlySelectedFieldInfo = null,
                LazyElementRefs = null,
                ElementRefs =  []
            };
        }

        var parameterType = action.MethodInfo.GetParameters().First().ParameterType;
        return state with
        {
            SelectedMethod = action.MethodInfo,
            ParameterValues = GetParameterProperties(parameterType, state.ModelId),
        };
    }

    private static ComplexFieldTypeMarker GetParameterProperties(
        Type parameterType,
        string modelId,
        bool? isRequired = null,
        int numSelectableFieldsCreated = 0
    )
    {
        int numItemsFilledIn = 0;
        ComplexFieldTypeMarker parameterProps = new(isRequired ?? true);
        foreach (
            PropertyInfo property in parameterType.GetProperties(
                BindingFlags.Public | BindingFlags.Instance
            )
        )
        {
            Type? nullableType = Nullable.GetUnderlyingType(property.PropertyType);
            if (SelectionInfoSingleItemComponent2.IsSimpleType(property.PropertyType))
            {
                parameterProps.Add2(
                    property.Name,
                    new SimpleFieldTypeMarker()
                    {
                        FieldType = property.PropertyType,
                        FieldNum = numSelectableFieldsCreated++,
                        IsRequired = true
                    }
                );
                if (property.Name == "ModelId")
                {
                    parameterProps.Set(property.Name, modelId);
                    numItemsFilledIn++;
                }
            }
            else if (
                nullableType is not null
                && SelectionInfoSingleItemComponent2.IsSimpleType(nullableType)
            )
            {
                parameterProps.Add2(
                    property.Name,
                    new SimpleFieldTypeMarker()
                    {
                        FieldType = nullableType,
                        FieldNum = numSelectableFieldsCreated++,
                        IsRequired = false
                    }
                );
            }
            else if (nullableType is not null)
            {
                parameterProps.Add2(
                    property.Name,
                    GetParameterProperties(nullableType, modelId, false, numSelectableFieldsCreated)
                );
            }
            else
            {
                parameterProps.Add2(
                    property.Name,
                    GetParameterProperties(
                        property.PropertyType,
                        modelId,
                        true,
                        numSelectableFieldsCreated
                    )
                );
            }
        }
        parameterProps.NumRecordsAutoFilled = numItemsFilledIn;
        return parameterProps;
    }

    //private static void AddSimpleFieldTypeMarker(ComplexFieldTypeMarker complex, PropertyInfo propertyInfo, string modelId, int numSelectableFieldsCreated)
    //{
    //    if (propertyInfo.Name == "ModelId")
    //    {
    //        complex.Add2(
    //                propertyInfo.Name,
    //                new SimpleFieldTypeMarker()
    //                {
    //                    FieldType = propertyInfo.PropertyType,
    //                    FieldNum = numSelectableFieldsCreated++,
    //                    IsRequired = true
    //                }
    //            );
    //        complex.Set(propertyInfo.Name, modelId);
    //        //numItemsFilledIn++;
    //    }
    //    else if (propertyInfo.Name == "MaterialId")
    //    {

    //    }
    //}
}

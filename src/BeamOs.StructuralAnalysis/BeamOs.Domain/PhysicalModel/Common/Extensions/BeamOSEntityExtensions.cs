using System.Reflection;
using BeamOs.Domain.Common.Interfaces;
using BeamOs.Domain.Common.ValueObjects;
using UnitsNet;

namespace BeamOs.Domain.PhysicalModel.Common.Extensions;

public static class BeamOSEntityExtensions
{
    public static void UseUnitSettings(this IBeamOsDomainObject entity, UnitSettings unitSettings)
    {
        UseUnitSettingsRecursive(entity, unitSettings, []);
    }

    private static void UseUnitSettingsRecursive(
        this IBeamOsDomainObject entity,
        UnitSettings unitSettings,
        HashSet<int> visitedObjectHashCodes
    )
    {
        int code = entity.GetHashCode();
        if (visitedObjectHashCodes.Contains(code))
        {
            return;
        }
        visitedObjectHashCodes.Add(code);

        foreach (
            var propInfo in entity
                .GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
        )
        {
            var type = Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType;
            var prop = propInfo.GetValue(entity, null);

            if (prop is IQuantity quantity)
            {
                foreach (var interfac in type.GetInterfaces())
                {
                    if (interfac.IsGenericType && interfac.GetGenericArguments().Length == 1)
                    {
                        var unitType = interfac.GetGenericArguments()[0];
                        Enum unit = unitSettings.GetUnit(unitType);
                        var convertedQuantity = quantity.ToUnit(unit);
                        propInfo.SetValue(entity, convertedQuantity, null);
                        break;
                    }
                }
            }
            else if (prop is IEnumerable<IBeamOsDomainObject> domainObjects)
            {
                foreach (var domainObject in domainObjects)
                {
                    UseUnitSettingsRecursive(domainObject, unitSettings, visitedObjectHashCodes);
                }
            }
            else if (prop is IBeamOsDomainObject domainObject)
            {
                domainObject.UseUnitSettingsRecursive(unitSettings, visitedObjectHashCodes);
            }
        }
    }
}

using System.Reflection;
using BeamOS.Common.Domain.Interfaces;
using BeamOS.PhysicalModel.Domain.ModelAggregate.ValueObjects;
using UnitsNet;

namespace BeamOS.PhysicalModel.Domain.Common.Extensions;

public static class BeamOSEntityExtensions
{
    public static void UseUnitSettings(this IBeamOsDomainObject entity, UnitSettings unitSettings)
    {
        foreach (PropertyInfo propInfo in entity.GetType().GetProperties())
        {
            var type = Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType;
            object? prop = propInfo.GetValue(entity, null);

            if (prop is IQuantity quantity)
            {
                foreach (Type interfac in type.GetInterfaces())
                {
                    if (interfac.IsGenericType && interfac.GetGenericArguments().Length == 1)
                    {
                        Type unitType = interfac.GetGenericArguments()[0];
                        Enum unit = unitSettings.GetUnit(unitType);
                        IQuantity convertedQuantity = quantity.ToUnit(unit);
                        propInfo.SetValue(entity, convertedQuantity, null);
                        break;
                    }
                }
            }
            else if (prop is IBeamOsDomainObject domainObject)
            {
                domainObject.UseUnitSettings(unitSettings);
            }
        }
    }
}

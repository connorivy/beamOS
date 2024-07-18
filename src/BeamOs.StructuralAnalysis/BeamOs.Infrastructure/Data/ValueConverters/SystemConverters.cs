using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BeamOs.Infrastructure.Data.ValueConverters;

public class DoubleArrayConverter : ValueConverter<double[], string>
{
    public DoubleArrayConverter()
        : base(
            x => string.Join(';', x),
            x => Array.ConvertAll(x.Split(';', StringSplitOptions.RemoveEmptyEntries), double.Parse)
        ) { }
}

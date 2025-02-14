using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BeamOs.StructuralAnalysis.Infrastructure.Common;

public class DoubleArrayConverter : ValueConverter<double[], string>
{
    public DoubleArrayConverter()
        : base(
            x => string.Join(';', x),
            x => Array.ConvertAll(x.Split(';', StringSplitOptions.RemoveEmptyEntries), double.Parse)
        ) { }
}

//public class DictStringObjConverter : ValueConverter<Dictionary<string, object?>, string>
//{
//    private static JsonSerializerOptions serializerOptions = new() { };

//    public DictStringObjConverter()
//        : base(
//            x => JsonSerializer.Serialize(x, serializerOptions),
//            x => JsonSerializer.Deserialize<Dictionary<string, object?>>(x, serializerOptions)
//        ) { }
//}

using System.Text.Json;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCases;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BeamOs.StructuralAnalysis.Infrastructure.Common;

internal class DoubleArrayConverter : ValueConverter<double[], string>
{
    public DoubleArrayConverter()
        : base(
            x => string.Join(';', x),
            x => Array.ConvertAll(x.Split(';', StringSplitOptions.RemoveEmptyEntries), double.Parse)
        ) { }
}

internal class DictIdToDoubleConverter : ValueConverter<Dictionary<LoadCaseId, double>, string>
{
    public DictIdToDoubleConverter()
        : base(
            static x =>
                JsonSerializer.Serialize(
                    x.ToDictionary(k => (int)k.Key, v => v.Value),
                    BeamOsJsonSerializerContext.Default.DictionaryInt32Double
                ),
            static x =>
                JsonSerializer
                    .Deserialize(x, BeamOsJsonSerializerContext.Default.DictionaryInt32Double)
                    .ToDictionary(k => new LoadCaseId(k.Key), v => v.Value)
        ) { }
}

internal class DictIntToDoubleConverter : ValueConverter<Dictionary<int, double>, string>
{
    public DictIntToDoubleConverter()
        : base(
            static x =>
                JsonSerializer.Serialize(
                    x,
                    BeamOsJsonSerializerContext.Default.DictionaryInt32Double
                ),
            static x =>
                JsonSerializer.Deserialize(
                    x,
                    BeamOsJsonSerializerContext.Default.DictionaryInt32Double
                )
        ) { }
}

// internal class Vector3Converter : ValueConverter<Vector3, string>
// {
//     public Vector3Converter()
//         : base(
//             static x => JsonSerializer.Serialize(x, BeamOsSerializerOptions.Default),
//             static x => JsonSerializer.Deserialize<Vector3>(x, BeamOsSerializerOptions.Default)
//         ) { }
// }

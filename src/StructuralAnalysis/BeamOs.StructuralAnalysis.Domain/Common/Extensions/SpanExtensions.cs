namespace BeamOs.StructuralAnalysis.Domain.Common.Extensions;

internal static class SpanExtensions
{
    public static void Fill<T>(this Span<T> span, params Span<T> values)
    {
        if (span.Length != values.Length)
        {
            throw new ArgumentException(
                "The length of the values span must be equal to the length of the target span."
            );
        }

        values.CopyTo(span);
    }
}

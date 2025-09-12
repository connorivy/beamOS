using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MathNet.Numerics.LinearAlgebra;

namespace BeamOs.StructuralAnalysis.Domain.DirectStiffnessMethod.Common.ValueObjects;

internal readonly record struct VectorIdentified
    : IEnumerable<KeyValuePair<UnsupportedStructureDisplacementId, double>>
{
    private readonly Dictionary<UnsupportedStructureDisplacementId, double> identifiers;

    public VectorIdentified(Dictionary<UnsupportedStructureDisplacementId, double> identifiers)
    {
        this.identifiers = identifiers;
    }

    public VectorIdentified(
        IList<UnsupportedStructureDisplacementId> identifiers,
        IList<double>? values = null
    )
        : this(CreateIdentifiers(identifiers, values)) { }

    public VectorIdentified(
        Span<UnsupportedStructureDisplacementId> identifiers,
        Span<double> values
    )
        : this(CreateIdentifiers(identifiers, values)) { }

    private static Dictionary<UnsupportedStructureDisplacementId, double> CreateIdentifiers(
        IList<UnsupportedStructureDisplacementId> identifiers,
        IList<double>? values
    )
    {
        Dictionary<UnsupportedStructureDisplacementId, double> identifierDict = new(
            identifiers.Count
        );

        for (int i = 0; i < identifiers.Count; i++)
        {
            identifierDict.Add(identifiers[i], values?.ElementAt(i) ?? 0);
        }

        return identifierDict;
    }

    private static Dictionary<UnsupportedStructureDisplacementId, double> CreateIdentifiers(
        Span<UnsupportedStructureDisplacementId> identifiers,
        Span<double> values
    )
    {
        Dictionary<UnsupportedStructureDisplacementId, double> identifierDict = new(
            identifiers.Length
        );

        for (int i = 0; i < identifiers.Length; i++)
        {
            identifierDict.Add(identifiers[i], values[i]);
        }

        return identifierDict;
    }

    public void AddEntriesWithMatchingIdentifiers(VectorIdentified vectorToBeAdded)
    {
        foreach (var kvp in vectorToBeAdded)
        {
            ref var valueRef = ref CollectionsMarshal.GetValueRefOrNullRef(
                this.identifiers,
                kvp.Key
            );

            if (!Unsafe.IsNullRef(ref valueRef))
            {
                valueRef += kvp.Value;
            }
        }
    }

    public Vector<double> Build()
    {
        return Vector<double>.Build.DenseOfEnumerable(this.identifiers.Values);
    }

    public IEnumerator<KeyValuePair<UnsupportedStructureDisplacementId, double>> GetEnumerator()
    {
        foreach (var kvp in this.identifiers)
        {
            yield return new KeyValuePair<UnsupportedStructureDisplacementId, double>(
                kvp.Key,
                kvp.Value
            );
        }
    }

    public double[] ToArray() => this.identifiers.Values.ToArray();

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}

internal readonly record struct VectorIdentifiedBase<TUnsupportedStructureDisplacementId2>
    : IEnumerable<KeyValuePair<TUnsupportedStructureDisplacementId2, double>>
    where TUnsupportedStructureDisplacementId2 : notnull
{
    private readonly Dictionary<TUnsupportedStructureDisplacementId2, double> identifiers;

    public VectorIdentifiedBase(
        Span<TUnsupportedStructureDisplacementId2> identifiers,
        Span<double> values
    )
    {
        this.identifiers = new(identifiers.Length);

        for (int i = 0; i < identifiers.Length; i++)
        {
            this.identifiers.Add(identifiers[i], values[i]);
        }
    }

    public VectorIdentifiedBase(Span<TUnsupportedStructureDisplacementId2> identifiers)
        : this(identifiers, Enumerable.Repeat(0.0, identifiers.Length).ToArray()) { }

    public void AddEntriesWithMatchingIdentifiers(
        VectorIdentifiedBase<TUnsupportedStructureDisplacementId2> vectorToBeAdded
    )
    {
        foreach (var kvp in vectorToBeAdded)
        {
            ref var valueRef = ref CollectionsMarshal.GetValueRefOrNullRef(
                this.identifiers,
                kvp.Key
            );

            if (!Unsafe.IsNullRef(ref valueRef))
            {
                valueRef += kvp.Value;
            }
        }
    }

    public Vector<double> Build()
    {
        return Vector<double>.Build.DenseOfEnumerable(this.identifiers.Values);
    }

    public IEnumerator<KeyValuePair<TUnsupportedStructureDisplacementId2, double>> GetEnumerator()
    {
        foreach (var kvp in this.identifiers)
        {
            yield return new KeyValuePair<TUnsupportedStructureDisplacementId2, double>(
                kvp.Key,
                kvp.Value
            );
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}

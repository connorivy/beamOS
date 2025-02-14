using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MathNet.Numerics.LinearAlgebra;

namespace BeamOs.StructuralAnalysis.Domain.DirectStiffnessMethod.Common.ValueObjects;

public readonly record struct VectorIdentified
    : IEnumerable<KeyValuePair<UnsupportedStructureDisplacementId, double>>
{
    private readonly Dictionary<UnsupportedStructureDisplacementId, double> identifiers;

    public VectorIdentified(
        IList<UnsupportedStructureDisplacementId> identifiers,
        IList<double>? values = null
    )
    {
        this.identifiers = new(identifiers.Count);

        for (int i = 0; i < identifiers.Count; i++)
        {
            this.identifiers.Add(identifiers[i], values?.ElementAt(i) ?? 0);
        }
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

public readonly record struct VectorIdentifiedBase<UnsupportedStructureDisplacementId2>
    : IEnumerable<KeyValuePair<UnsupportedStructureDisplacementId2, double>>
    where UnsupportedStructureDisplacementId2 : notnull
{
    private readonly Dictionary<UnsupportedStructureDisplacementId2, double> identifiers;

    public VectorIdentifiedBase(
        Span<UnsupportedStructureDisplacementId2> identifiers,
        Span<double> values
    )
    {
        this.identifiers = new(identifiers.Length);

        for (int i = 0; i < identifiers.Length; i++)
        {
            this.identifiers.Add(identifiers[i], values[i]);
        }
    }

    public VectorIdentifiedBase(Span<UnsupportedStructureDisplacementId2> identifiers)
        : this(identifiers, Enumerable.Repeat(0.0, identifiers.Length).ToArray()) { }

    public void AddEntriesWithMatchingIdentifiers(
        VectorIdentifiedBase<UnsupportedStructureDisplacementId2> vectorToBeAdded
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

    public IEnumerator<KeyValuePair<UnsupportedStructureDisplacementId2, double>> GetEnumerator()
    {
        foreach (var kvp in this.identifiers)
        {
            yield return new KeyValuePair<UnsupportedStructureDisplacementId2, double>(
                kvp.Key,
                kvp.Value
            );
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}

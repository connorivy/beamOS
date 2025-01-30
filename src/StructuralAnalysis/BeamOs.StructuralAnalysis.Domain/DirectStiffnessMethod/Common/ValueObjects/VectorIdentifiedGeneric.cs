using System.Collections;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace BeamOs.StructuralAnalysis.Domain.DirectStiffnessMethod.Common.ValueObjects;

public class VectorIdentifiedGeneric<TIdentifier> : IEnumerable<KeyValuePair<TIdentifier, double>>
    where TIdentifier : notnull
{
    private readonly List<TIdentifier> identifiers;
    public double[] Values { get; }

    public VectorIdentifiedGeneric(List<TIdentifier> identifiers, double[]? values)
    {
        this.identifiers = identifiers;
        this.Values = values ?? new double[identifiers.Count];
    }

    public void AddEntriesWithMatchingIdentifiers(
        VectorIdentifiedGeneric<TIdentifier> vectorToBeAdded
    )
    {
        for (var incomingIndex = 0; incomingIndex < vectorToBeAdded.Values.Length; incomingIndex++)
        {
            var incomingIdentifier = vectorToBeAdded.identifiers[incomingIndex];

            var thisIdentifierIndex = this.identifiers.FindIndex(i => i.Equals(incomingIdentifier));
            if (thisIdentifierIndex == -1)
            {
                continue;
            }

            this.Values[thisIdentifierIndex] += vectorToBeAdded.Values[incomingIndex];
        }
    }

    public Vector<double> Build()
    {
        return DenseVector.OfArray(this.Values);
    }

    public IEnumerator<KeyValuePair<TIdentifier, double>> GetEnumerator()
    {
        for (var i = 0; i < this.identifiers.Count; i++)
        {
            yield return new KeyValuePair<TIdentifier, double>(this.identifiers[i], this.Values[i]);
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}

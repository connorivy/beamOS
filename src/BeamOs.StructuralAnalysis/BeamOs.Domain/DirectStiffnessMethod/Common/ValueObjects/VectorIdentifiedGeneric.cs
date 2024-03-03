using System.Collections;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace BeamOs.Domain.DirectStiffnessMethod.Common.ValueObjects;

public class VectorIdentifiedGeneric<TIdentifier> : IEnumerable<KeyValuePair<TIdentifier, double>>
    where TIdentifier : notnull
{
    private readonly List<TIdentifier> identifiers;
    private readonly double[] values;

    public VectorIdentifiedGeneric(List<TIdentifier> identifiers, double[]? values)
    {
        this.identifiers = identifiers;
        this.values = values ?? new double[identifiers.Count];
    }

    public void AddEntriesWithMatchingIdentifiers(
        VectorIdentifiedGeneric<TIdentifier> vectorToBeAdded
    )
    {
        for (var incomingIndex = 0; incomingIndex < vectorToBeAdded.values.Length; incomingIndex++)
        {
            var incomingIdentifier = vectorToBeAdded.identifiers[incomingIndex];

            var thisIdentifierIndex = this.identifiers.FindIndex(i => i.Equals(incomingIdentifier));
            if (thisIdentifierIndex == -1)
            {
                continue;
            }

            this.values[thisIdentifierIndex] += vectorToBeAdded.values[incomingIndex];
        }
    }

    public Vector<double> Build()
    {
        return DenseVector.OfArray(this.values);
    }

    public IEnumerator<KeyValuePair<TIdentifier, double>> GetEnumerator()
    {
        for (var i = 0; i < this.identifiers.Count; i++)
        {
            yield return new KeyValuePair<TIdentifier, double>(this.identifiers[i], this.values[i]);
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}

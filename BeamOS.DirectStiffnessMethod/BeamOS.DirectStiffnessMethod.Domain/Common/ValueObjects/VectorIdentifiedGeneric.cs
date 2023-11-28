using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace BeamOS.DirectStiffnessMethod.Domain.Common.ValueObjects;
public class VectorIdentifiedGeneric<TIdentifier>
    where TIdentifier : notnull
{
    private readonly List<TIdentifier> identifiers;
    private readonly double[] values;
    public VectorIdentifiedGeneric(List<TIdentifier> identifiers, double[]? values)
    {
        this.identifiers = identifiers;
        this.values = values ?? new double[identifiers.Count];
    }

    public void AddEntriesWithMatchingIdentifiers(VectorIdentifiedGeneric<TIdentifier> vectorToBeAdded)
    {
        for (int incomingIndex = 0; incomingIndex < vectorToBeAdded.values.Length; incomingIndex++)
        {
            TIdentifier incomingIdentifier = vectorToBeAdded.identifiers[incomingIndex];

            int thisIdentifierIndex = this.identifiers.FindIndex(i => i.Equals(incomingIdentifier));
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
}

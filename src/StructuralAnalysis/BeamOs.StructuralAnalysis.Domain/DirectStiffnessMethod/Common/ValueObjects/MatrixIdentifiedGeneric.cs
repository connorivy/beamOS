using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace BeamOs.StructuralAnalysis.Domain.DirectStiffnessMethod.Common.ValueObjects;

public class MatrixIdentifiedGeneric<TIdentifier>
{
    private readonly Dictionary<TIdentifier, int> rowIdentifierToIndexDict = [];
    private readonly List<TIdentifier> rowIdentifiers;
    private readonly List<TIdentifier> columnIdentifiers;
    public double[,] Values { get; }

    public MatrixIdentifiedGeneric(List<TIdentifier> identifiers, double[,]? values = null)
    {
        this.rowIdentifiers = identifiers;
        for (int i = 0; i < identifiers.Count; i++)
        {
            this.rowIdentifierToIndexDict.Add(identifiers[i], i);
        }
        this.columnIdentifiers = this.rowIdentifiers;
        this.Values = values ?? new double[identifiers.Count, identifiers.Count];
    }

    public double? GetValue(TIdentifier rowIdentifier, TIdentifier columnIdentifier)
    {
        var rowIndex = this.rowIdentifiers.FindIndex(i => i.Equals(rowIdentifier));
        var columnIndex = this.columnIdentifiers.FindIndex(i => i.Equals(columnIdentifier));

        if (rowIndex < 0 || columnIndex < 0)
        {
            return null;
        }

        return this.Values[rowIndex, columnIndex];
    }

    public void SetValue(TIdentifier rowIdentifier, TIdentifier columnIdentifier, double value)
    {
        var rowIndex = this.rowIdentifiers.FindIndex(i => i.Equals(rowIdentifier));
        var columnIndex = this.columnIdentifiers.FindIndex(i => i.Equals(columnIdentifier));

        this.Values[rowIndex, columnIndex] = value;
    }

    // TODO : could optimize for symmetric matrices
    public void AddEntriesWithMatchingIdentifiers(
        MatrixIdentifiedGeneric<TIdentifier> matrixToBeAdded
    )
    {
        for (
            var incomingRowIndex = 0;
            incomingRowIndex < matrixToBeAdded.Values.GetLength(0);
            incomingRowIndex++
        )
        {
            var incomingRowIdentifier = matrixToBeAdded.rowIdentifiers[incomingRowIndex];

            if (
                !this.rowIdentifierToIndexDict.TryGetValue(
                    incomingRowIdentifier,
                    out var thisRowIndex
                )
            )
            {
                continue;
            }

            //var thisRowIndex = this.rowIdentifiers.FindIndex(i => i.Equals(incomingRowIdentifier));
            //if (thisRowIndex == -1)
            //{
            //    continue;
            //}

            for (
                var incomingColumnIndex = 0;
                incomingColumnIndex < matrixToBeAdded.Values.GetLength(1);
                incomingColumnIndex++
            )
            {
                var incomingColumnIdentifier = matrixToBeAdded.rowIdentifiers[incomingColumnIndex];

                if (
                    !this.rowIdentifierToIndexDict.TryGetValue(
                        incomingColumnIdentifier,
                        out var thisColumnIndex
                    )
                )
                {
                    continue;
                }

                //var thisColumnIndex = this.rowIdentifiers.FindIndex(
                //    i => i.Equals(incomingColumnIdentifier)
                //);
                //if (thisColumnIndex == -1)
                //{
                //    continue;
                //}

                this.Values[thisRowIndex, thisColumnIndex] += matrixToBeAdded.Values[
                    incomingRowIndex,
                    incomingColumnIndex
                ];
            }
        }
    }

    public Matrix<double> Build()
    {
        return DenseMatrix.OfArray(this.Values);
    }
}

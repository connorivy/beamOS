using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace BeamOS.DirectStiffnessMethod.Domain.Common.ValueObjects;

public class MatrixIdentifiedGeneric<TIdentifier>
{
    private readonly List<TIdentifier> rowIdentifiers;
    private readonly List<TIdentifier> columnIdentifiers;
    private readonly double[,] values;

    public MatrixIdentifiedGeneric(List<TIdentifier> identifiers, double[,]? values = null)
    {
        this.rowIdentifiers = identifiers;
        this.columnIdentifiers = this.rowIdentifiers;
        this.values = values ?? new double[identifiers.Count, identifiers.Count];
    }

    public double? GetValue(TIdentifier rowIdentifier, TIdentifier columnIdentifier)
    {
        int rowIndex = this.rowIdentifiers.FindIndex(i => i.Equals(rowIdentifier));
        int columnIndex = this.columnIdentifiers.FindIndex(i => i.Equals(columnIdentifier));

        if (rowIndex < 0 || columnIndex < 0)
        {
            return null;
        }

        return this.values[rowIndex, columnIndex];
    }

    public void SetValue(TIdentifier rowIdentifier, TIdentifier columnIdentifier, double value)
    {
        int rowIndex = this.rowIdentifiers.FindIndex(i => i.Equals(rowIdentifier));
        int columnIndex = this.columnIdentifiers.FindIndex(i => i.Equals(columnIdentifier));

        this.values[rowIndex, columnIndex] = value;
    }

    // TODO : could optimize for symmetric matrices
    public void AddEntriesWithMatchingIdentifiers(
        MatrixIdentifiedGeneric<TIdentifier> matrixToBeAdded
    )
    {
        for (
            int incomingRowIndex = 0;
            incomingRowIndex < matrixToBeAdded.values.GetLength(0);
            incomingRowIndex++
        )
        {
            TIdentifier incomingRowIdentifier = matrixToBeAdded.rowIdentifiers[incomingRowIndex];

            int thisRowIndex = this.rowIdentifiers.FindIndex(i => i.Equals(incomingRowIdentifier));
            if (thisRowIndex == -1)
            {
                continue;
            }

            for (
                int incomingColumnIndex = 0;
                incomingColumnIndex < matrixToBeAdded.values.GetLength(1);
                incomingColumnIndex++
            )
            {
                TIdentifier incomingColumnIdentifier = matrixToBeAdded.rowIdentifiers[
                    incomingColumnIndex
                ];

                int thisColumnIndex = this.rowIdentifiers.FindIndex(
                    i => i.Equals(incomingColumnIdentifier)
                );
                if (thisColumnIndex == -1)
                {
                    continue;
                }

                this.values[thisRowIndex, thisColumnIndex] += matrixToBeAdded.values[
                    incomingRowIndex,
                    incomingColumnIndex
                ];
            }
        }
    }

    public Matrix<double> Build()
    {
        return DenseMatrix.OfArray(this.values);
    }
}

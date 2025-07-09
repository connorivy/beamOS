using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using FluentAssertions;

namespace BeamOs.Tests.Common;

public class Asserter
{
    public static event EventHandler<ComparedObjectEventArgs<double>>? DoublesAssertedEqual;
    public static event EventHandler<ComparedObjectEventArgs>? AssertedEqual2;
    public static event EventHandler<ComparedObjectEventArgs<double[]>>? DoubleArrayAssertedEqual;
    public static event EventHandler<
        ComparedObjectEventArgs<double[,]>
    >? Double2dArrayAssertedEqual;

    public void AssertEqual(
        BeamOsObjectType beamOsObjectType,
        string beamOsObjectId,
        string comparedValueName,
        double[] expected,
        double[] actual,
        double precision = .001,
        ICollection<string>? comparedValueNameCollection = null
    )
    {
        AssertedEqual2?.Invoke(
            typeof(Asserter),
            new()
            {
                BeamOsObjectType = beamOsObjectType,
                BeamOsObjectId = beamOsObjectId,
                ComparedObjectPropertyName = comparedValueName,
                ExpectedValue = expected,
                CalculatedValue = actual,
                ComparedValueNameCollection = comparedValueNameCollection,
            }
        );

        if (actual.Length != expected.Length)
        {
            throw new Exception("Calculated and expected values have different lengths");
        }

        if (actual.Length == 0)
        {
            return;
        }

        for (int col = 0; col < actual.Length; col++)
        {
            actual[col].Should().BeApproximately(expected[col], precision);
        }
    }

    public void AssertEqual(
        BeamOsObjectType beamOsObjectType,
        string beamOsObjectId,
        string comparedValueName,
        double?[] expected,
        double?[] actual,
        double precision = .001,
        ICollection<string>? comparedValueNameCollection = null
    )
    {
        AssertedEqual2?.Invoke(
            typeof(Asserter),
            new()
            {
                BeamOsObjectType = beamOsObjectType,
                BeamOsObjectId = beamOsObjectId,
                ComparedObjectPropertyName = comparedValueName,
                ExpectedValue = expected,
                CalculatedValue = actual,
                ComparedValueNameCollection = comparedValueNameCollection,
            }
        );

        if (actual.Length != expected.Length)
        {
            throw new Exception("Calculated and expected values have different lengths");
        }

        if (actual.Length == 0)
        {
            return;
        }

        for (int col = 0; col < actual.Length; col++)
        {
            double? actualValue = actual[col];
            double? expectedValue = expected[col];
            if (expectedValue is null)
            {
                continue;
            }

            actualValue.Should().NotBeNull();
            actualValue!.Value.Should().BeApproximately(expectedValue.Value, precision);
        }
    }

    public void AssertEqual(
        BeamOsObjectType beamOsObjectType,
        string beamOsObjectId,
        string comparedValueName,
        double[,] expected,
        double[,] actual,
        double precision = .001,
        ICollection<string>? comparedValueNameCollection = null
    )
    {
        AssertedEqual2?.Invoke(
            typeof(Asserter),
            new()
            {
                BeamOsObjectType = beamOsObjectType,
                BeamOsObjectId = beamOsObjectId,
                ComparedObjectPropertyName = comparedValueName,
                ExpectedValue = expected,
                CalculatedValue = actual,
                ComparedValueNameCollection = comparedValueNameCollection,
            }
        );

        int numRows = actual.GetLength(0);
        int numCols = actual.GetLength(1);
        if (numRows != expected.GetLength(0) || numCols != expected.GetLength(1))
        {
            throw new Exception("Calculated and expected values have different lengths");
        }

        if (numRows == 0 || numCols == 0)
        {
            return;
        }

        for (int row = 0; row < numRows; row++)
        {
            for (int col = 0; col < numCols; col++)
            {
                actual[row, col].Should().BeApproximately(expected[row, col], precision);
            }
        }
    }

    public event EventHandler<ModelProposalResponse>? ModelProposalVerified;

    public virtual ValueTask VerifyModelProposal(
        Result<ModelProposalResponse> modelProposalResponse
    )
    {
        if (modelProposalResponse.IsSuccess)
        {
            ModelProposalVerified?.Invoke(typeof(Asserter), modelProposalResponse.Value);
        }
        return ValueTask.CompletedTask;
    }
}

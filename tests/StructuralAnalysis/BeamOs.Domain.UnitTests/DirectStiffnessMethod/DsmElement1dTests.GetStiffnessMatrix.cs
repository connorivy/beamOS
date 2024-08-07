using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.DirectStiffnessMethod;
using BeamOs.Domain.IntegrationTests.DirectStiffnessMethod.Common.Fixtures;
using BeamOs.Domain.IntegrationTests.DirectStiffnessMethod.Common.SolvedProblems;
using BeamOs.Domain.UnitTests.DirectStiffnessMethod.Common.Factories;
using BeamOS.Tests.Common;
using BeamOS.Tests.Common.Traits;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.Domain.UnitTests.DirectStiffnessMethod;

[DirectStiffnessMethod]
public partial class DsmElement1dTests
{
    [SkippableTheory]
    [ClassData(typeof(AllDsmElement1dFixtures))]
    public void GetLocalStiffnessMatrix_ForAllElement1DFixtures_ShouldEqualExpectedValue(
        DsmElement1dFixture fixture
    )
    {
        Skip.If(fixture.ExpectedLocalStiffnessMatrix is null);

        double[,] stiffnessMatrix = fixture
            .ToDomain()
            .GetLocalStiffnessMatrix(
                fixture.Model.Value.Settings.UnitSettings.ForceUnit.MapToForceUnit(),
                fixture
                    .Model
                    .Value
                    .Settings
                    .UnitSettings
                    .ForcePerLengthUnit
                    .MapToForcePerLengthUnit(),
                fixture.Model.Value.Settings.UnitSettings.TorqueUnit.MapToTorqueUnit()
            )
            .ToArray();

        Asserter.AssertEqual(
            "Local Stiffness Matrix",
            fixture.ExpectedLocalStiffnessMatrix,
            stiffnessMatrix,
            1
        );
    }

    [SkippableTheory]
    [ClassData(typeof(AllDsmElement1dFixtures))]
    public void GetGlobalStiffnessMatrix_ForAllElement1DFixtures_ShouldEqualExpectedValue(
        DsmElement1dFixture fixture
    )
    {
        Skip.If(fixture.ExpectedGlobalStiffnessMatrix is null);

        double[,] stiffnessMatrix = fixture
            .ToDomain()
            .GetGlobalStiffnessMatrix(
                fixture.Model.Value.Settings.UnitSettings.ForceUnit.MapToForceUnit(),
                fixture
                    .Model
                    .Value
                    .Settings
                    .UnitSettings
                    .ForcePerLengthUnit
                    .MapToForcePerLengthUnit(),
                fixture.Model.Value.Settings.UnitSettings.TorqueUnit.MapToTorqueUnit()
            )
            .ToArray();

        Asserter.AssertEqual(
            "Global Stiffness Matrix",
            fixture.ExpectedGlobalStiffnessMatrix,
            stiffnessMatrix,
            1
        );
    }

    [Fact]
    public void GetStiffnessMatrix_WithAllUnitValue_ShouldEqualCoefficients()
    {
        //AnalyticalElement1D element = Element1DFactory.Create();
        DsmElement1d element = DsmElement1dFactory.CreateWithUnitSiValues();

        double[,] stiffnessMatrix = element
            .GetLocalStiffnessMatrix(
                UnitSettings.SI.ForceUnit,
                UnitSettings.SI.ForcePerLengthUnit,
                UnitSettings.SI.TorqueUnit
            )
            .ToArray();

        double[,] expectedLocalStiffnessMatrix = new double[12, 12]
        {
            { 1, 0, 0, 0, 0, 0, -1, 0, 0, 0, 0, 0 },
            { 0, 12, 0, 0, 0, 6, 0, -12, 0, 0, 0, 6 },
            { 0, 0, 12, 0, -6, 0, 0, 0, -12, 0, -6, 0 },
            { 0, 0, 0, 1, 0, 0, 0, 0, 0, -1, 0, 0 },
            { 0, 0, -6, 0, 4, 0, 0, 0, 6, 0, 2, 0 },
            { 0, 6, 0, 0, 0, 4, 0, -6, 0, 0, 0, 2 },
            { -1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 },
            { 0, -12, 0, 0, 0, -6, 0, 12, 0, 0, 0, -6 },
            { 0, 0, -12, 0, 6, 0, 0, 0, 12, 0, 6, 0 },
            { 0, 0, 0, -1, 0, 0, 0, 0, 0, 1, 0, 0 },
            { 0, 0, -6, 0, 2, 0, 0, 0, 6, 0, 4, 0 },
            { 0, 6, 0, 0, 0, 2, 0, -6, 0, 0, 0, 4 },
        };

        Asserter.AssertEqual("Stiffness Matrix", expectedLocalStiffnessMatrix, stiffnessMatrix);
    }

    [Fact]
    public void GetStiffnessMatrix_WithIsolatedEVariable_ShouldEqualExpectedValue()
    {
        DsmElement1d element = DsmElement1dFactory.CreateWithUnitSiValues(
            modulusOfElasticity: new(5, UnitSystem.SI)
        );

        double[,] stiffnessMatrix = element
            .GetLocalStiffnessMatrix(
                UnitSettings.SI.ForceUnit,
                UnitSettings.SI.ForcePerLengthUnit,
                UnitSettings.SI.TorqueUnit
            )
            .ToArray();

        double[,] expectedLocalStiffnessMatrix = new double[12, 12]
        {
            { 5 * 1, 0, 0, 0, 0, 0, -5 * 1, 0, 0, 0, 0, 0 },
            { 0, 5 * 12, 0, 0, 0, 5 * 6, 0, -5 * 12, 0, 0, 0, 5 * 6 },
            { 0, 0, 5 * 12, 0, -5 * 6, 0, 0, 0, -5 * 12, 0, -5 * 6, 0 },
            { 0, 0, 0, 1, 0, 0, 0, 0, 0, -1, 0, 0 },
            { 0, 0, -5 * 6, 0, 5 * 4, 0, 0, 0, 5 * 6, 0, 5 * 2, 0 },
            { 0, 5 * 6, 0, 0, 0, 5 * 4, 0, -5 * 6, 0, 0, 0, 5 * 2 },
            { -5 * 1, 0, 0, 0, 0, 0, 5 * 1, 0, 0, 0, 0, 0 },
            { 0, -5 * 12, 0, 0, 0, -5 * 6, 0, 5 * 12, 0, 0, 0, -5 * 6 },
            { 0, 0, -5 * 12, 0, 5 * 6, 0, 0, 0, 5 * 12, 0, 5 * 6, 0 },
            { 0, 0, 0, -1, 0, 0, 0, 0, 0, 1, 0, 0 },
            { 0, 0, -5 * 6, 0, 5 * 2, 0, 0, 0, 5 * 6, 0, 5 * 4, 0 },
            { 0, 5 * 6, 0, 0, 0, 5 * 2, 0, -5 * 6, 0, 0, 0, 5 * 4 },
        };

        Asserter.AssertEqual("Stiffness Matrix", expectedLocalStiffnessMatrix, stiffnessMatrix);
    }

    [Fact]
    public void GetStiffnessMatrix_WithIsolatedGVariable_ShouldEqualExpectedValue()
    {
        DsmElement1d element = DsmElement1dFactory.CreateWithUnitSiValues(
            modulusOfRigidity: new(5, UnitSystem.SI)
        );

        double[,] stiffnessMatrix = element
            .GetLocalStiffnessMatrix(
                UnitSettings.SI.ForceUnit,
                UnitSettings.SI.ForcePerLengthUnit,
                UnitSettings.SI.TorqueUnit
            )
            .ToArray();

        double[,] expectedLocalStiffnessMatrix = new double[12, 12]
        {
            { 1, 0, 0, 0, 0, 0, -1, 0, 0, 0, 0, 0 },
            { 0, 12, 0, 0, 0, 6, 0, -12, 0, 0, 0, 6 },
            { 0, 0, 12, 0, -6, 0, 0, 0, -12, 0, -6, 0 },
            { 0, 0, 0, 5 * 1, 0, 0, 0, 0, 0, -5 * 1, 0, 0 },
            { 0, 0, -6, 0, 4, 0, 0, 0, 6, 0, 2, 0 },
            { 0, 6, 0, 0, 0, 4, 0, -6, 0, 0, 0, 2 },
            { -1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 },
            { 0, -12, 0, 0, 0, -6, 0, 12, 0, 0, 0, -6 },
            { 0, 0, -12, 0, 6, 0, 0, 0, 12, 0, 6, 0 },
            { 0, 0, 0, -5 * 1, 0, 0, 0, 0, 0, 5 * 1, 0, 0 },
            { 0, 0, -6, 0, 2, 0, 0, 0, 6, 0, 4, 0 },
            { 0, 6, 0, 0, 0, 2, 0, -6, 0, 0, 0, 4 },
        };

        Asserter.AssertEqual("Stiffness Matrix", expectedLocalStiffnessMatrix, stiffnessMatrix);
    }

    [Fact]
    public void GetStiffnessMatrix_WithIsolatedAVariable_ShouldEqualExpectedValue()
    {
        DsmElement1d element = DsmElement1dFactory.CreateWithUnitSiValues(
            area: new(5, UnitSystem.SI)
        );

        double[,] stiffnessMatrix = element
            .GetLocalStiffnessMatrix(
                UnitSettings.SI.ForceUnit,
                UnitSettings.SI.ForcePerLengthUnit,
                UnitSettings.SI.TorqueUnit
            )
            .ToArray();

        double[,] expectedLocalStiffnessMatrix = new double[12, 12]
        {
            { 5 * 1, 0, 0, 0, 0, 0, -5 * 1, 0, 0, 0, 0, 0 },
            { 0, 12, 0, 0, 0, 6, 0, -12, 0, 0, 0, 6 },
            { 0, 0, 12, 0, -6, 0, 0, 0, -12, 0, -6, 0 },
            { 0, 0, 0, 1, 0, 0, 0, 0, 0, -1, 0, 0 },
            { 0, 0, -6, 0, 4, 0, 0, 0, 6, 0, 2, 0 },
            { 0, 6, 0, 0, 0, 4, 0, -6, 0, 0, 0, 2 },
            { -5 * 1, 0, 0, 0, 0, 0, 5 * 1, 0, 0, 0, 0, 0 },
            { 0, -12, 0, 0, 0, -6, 0, 12, 0, 0, 0, -6 },
            { 0, 0, -12, 0, 6, 0, 0, 0, 12, 0, 6, 0 },
            { 0, 0, 0, -1, 0, 0, 0, 0, 0, 1, 0, 0 },
            { 0, 0, -6, 0, 2, 0, 0, 0, 6, 0, 4, 0 },
            { 0, 6, 0, 0, 0, 2, 0, -6, 0, 0, 0, 4 },
        };

        Asserter.AssertEqual("Stiffness Matrix", expectedLocalStiffnessMatrix, stiffnessMatrix);
    }

    [Fact]
    public void GetStiffnessMatrix_WithIsolatedIStrongVariable_ShouldEqualExpectedValue()
    {
        DsmElement1d element = DsmElement1dFactory.CreateWithUnitSiValues(
            strongAxisMomentOfInertia: new(5, UnitSystem.SI)
        );

        double[,] stiffnessMatrix = element
            .GetLocalStiffnessMatrix(
                UnitSettings.SI.ForceUnit,
                UnitSettings.SI.ForcePerLengthUnit,
                UnitSettings.SI.TorqueUnit
            )
            .ToArray();

        double[,] expectedLocalStiffnessMatrix = new double[12, 12]
        {
            { 1, 0, 0, 0, 0, 0, -1, 0, 0, 0, 0, 0 },
            { 0, 5 * 12, 0, 0, 0, 5 * 6, 0, -5 * 12, 0, 0, 0, 5 * 6 },
            { 0, 0, 12, 0, -6, 0, 0, 0, -12, 0, -6, 0 },
            { 0, 0, 0, 1, 0, 0, 0, 0, 0, -1, 0, 0 },
            { 0, 0, -6, 0, 4, 0, 0, 0, 6, 0, 2, 0 },
            { 0, 5 * 6, 0, 0, 0, 5 * 4, 0, -5 * 6, 0, 0, 0, 5 * 2 },
            { -1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 },
            { 0, -5 * 12, 0, 0, 0, -5 * 6, 0, 5 * 12, 0, 0, 0, -5 * 6 },
            { 0, 0, -12, 0, 6, 0, 0, 0, 12, 0, 6, 0 },
            { 0, 0, 0, -1, 0, 0, 0, 0, 0, 1, 0, 0 },
            { 0, 0, -6, 0, 2, 0, 0, 0, 6, 0, 4, 0 },
            { 0, 5 * 6, 0, 0, 0, 5 * 2, 0, -5 * 6, 0, 0, 0, 5 * 4 },
        };

        Asserter.AssertEqual("Stiffness Matrix", expectedLocalStiffnessMatrix, stiffnessMatrix);
    }

    [Fact]
    public void GetStiffnessMatrix_WithIsolatedIWeakVariable_ShouldEqualExpectedValue()
    {
        DsmElement1d element = DsmElement1dFactory.CreateWithUnitSiValues(
            weakAxisMomentOfInertia: new(5, UnitSystem.SI)
        );

        double[,] stiffnessMatrix = element
            .GetLocalStiffnessMatrix(
                UnitSettings.SI.ForceUnit,
                UnitSettings.SI.ForcePerLengthUnit,
                UnitSettings.SI.TorqueUnit
            )
            .ToArray();

        double[,] expectedLocalStiffnessMatrix = new double[12, 12]
        {
            { 1, 0, 0, 0, 0, 0, -1, 0, 0, 0, 0, 0 },
            { 0, 12, 0, 0, 0, 6, 0, -12, 0, 0, 0, 6 },
            { 0, 0, 5 * 12, 0, -5 * 6, 0, 0, 0, -5 * 12, 0, -5 * 6, 0 },
            { 0, 0, 0, 1, 0, 0, 0, 0, 0, -1, 0, 0 },
            { 0, 0, -5 * 6, 0, 5 * 4, 0, 0, 0, 5 * 6, 0, 5 * 2, 0 },
            { 0, 6, 0, 0, 0, 4, 0, -6, 0, 0, 0, 2 },
            { -1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 },
            { 0, -12, 0, 0, 0, -6, 0, 12, 0, 0, 0, -6 },
            { 0, 0, -5 * 12, 0, 5 * 6, 0, 0, 0, 5 * 12, 0, 5 * 6, 0 },
            { 0, 0, 0, -1, 0, 0, 0, 0, 0, 1, 0, 0 },
            { 0, 0, -5 * 6, 0, 5 * 2, 0, 0, 0, 5 * 6, 0, 5 * 4, 0 },
            { 0, 6, 0, 0, 0, 2, 0, -6, 0, 0, 0, 4 },
        };

        Asserter.AssertEqual("Stiffness Matrix", expectedLocalStiffnessMatrix, stiffnessMatrix);
    }

    [Fact]
    public void GetStiffnessMatrix_WithIsolatedJVariable_ShouldEqualExpectedValue()
    {
        DsmElement1d element = DsmElement1dFactory.CreateWithUnitSiValues(
            polarMomentOfInertia: new(5, UnitSystem.SI)
        );

        double[,] stiffnessMatrix = element
            .GetLocalStiffnessMatrix(
                UnitSettings.SI.ForceUnit,
                UnitSettings.SI.ForcePerLengthUnit,
                UnitSettings.SI.TorqueUnit
            )
            .ToArray();

        double[,] expectedLocalStiffnessMatrix = new double[12, 12]
        {
            { 1, 0, 0, 0, 0, 0, -1, 0, 0, 0, 0, 0 },
            { 0, 12, 0, 0, 0, 6, 0, -12, 0, 0, 0, 6 },
            { 0, 0, 12, 0, -6, 0, 0, 0, -12, 0, -6, 0 },
            { 0, 0, 0, 5 * 1, 0, 0, 0, 0, 0, -5 * 1, 0, 0 },
            { 0, 0, -6, 0, 4, 0, 0, 0, 6, 0, 2, 0 },
            { 0, 6, 0, 0, 0, 4, 0, -6, 0, 0, 0, 2 },
            { -1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 },
            { 0, -12, 0, 0, 0, -6, 0, 12, 0, 0, 0, -6 },
            { 0, 0, -12, 0, 6, 0, 0, 0, 12, 0, 6, 0 },
            { 0, 0, 0, -5 * 1, 0, 0, 0, 0, 0, 5 * 1, 0, 0 },
            { 0, 0, -6, 0, 2, 0, 0, 0, 6, 0, 4, 0 },
            { 0, 6, 0, 0, 0, 2, 0, -6, 0, 0, 0, 4 },
        };

        Asserter.AssertEqual("Stiffness Matrix", expectedLocalStiffnessMatrix, stiffnessMatrix);
    }

    [Fact]
    public void GetStiffnessMatrix_WithIsolatedLVariable_ShouldEqualExpectedValue()
    {
        DsmElement1d element = DsmElement1dFactory.CreateWithUnitSiValues(
            baseLine: new(0, 0, 0, 5, 0, 0, LengthUnit.Meter)
        );

        double[,] stiffnessMatrix = element
            .GetLocalStiffnessMatrix(
                UnitSettings.SI.ForceUnit,
                UnitSettings.SI.ForcePerLengthUnit,
                UnitSettings.SI.TorqueUnit
            )
            .ToArray();

        double[,] expectedLocalStiffnessMatrix = new double[12, 12]
        {
            { 1 / 5.0, 0, 0, 0, 0, 0, -1 / 5.0, 0, 0, 0, 0, 0 },
            { 0, 12 / 125.0, 0, 0, 0, 6 / 25.0, 0, -12 / 125.0, 0, 0, 0, 6 / 25.0 },
            { 0, 0, 12 / 125.0, 0, -6 / 25.0, 0, 0, 0, -12 / 125.0, 0, -6 / 25.0, 0 },
            { 0, 0, 0, 1 / 5.0, 0, 0, 0, 0, 0, -1 / 5.0, 0, 0 },
            { 0, 0, -6 / 25.0, 0, 4 / 5.0, 0, 0, 0, 6 / 25.0, 0, 2 / 5.0, 0 },
            { 0, 6 / 25.0, 0, 0, 0, 4 / 5.0, 0, -6 / 25.0, 0, 0, 0, 2 / 5.0 },
            { -1 / 5.0, 0, 0, 0, 0, 0, 1 / 5.0, 0, 0, 0, 0, 0 },
            { 0, -12 / 125.0, 0, 0, 0, -6 / 25.0, 0, 12 / 125.0, 0, 0, 0, -6 / 25.0 },
            { 0, 0, -12 / 125.0, 0, 6 / 25.0, 0, 0, 0, 12 / 125.0, 0, 6 / 25.0, 0 },
            { 0, 0, 0, -1 / 5.0, 0, 0, 0, 0, 0, 1 / 5.0, 0, 0 },
            { 0, 0, -6 / 25.0, 0, 2 / 5.0, 0, 0, 0, 6 / 25.0, 0, 4 / 5.0, 0 },
            { 0, 6 / 25.0, 0, 0, 0, 2 / 5.0, 0, -6 / 25.0, 0, 0, 0, 4 / 5.0 },
        };

        Asserter.AssertEqual("Stiffness Matrix", expectedLocalStiffnessMatrix, stiffnessMatrix);
    }
}

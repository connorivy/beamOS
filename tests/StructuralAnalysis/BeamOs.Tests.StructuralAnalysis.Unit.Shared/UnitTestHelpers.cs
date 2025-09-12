using BeamOs.StructuralAnalysis.Domain.DirectStiffnessMethod;
using BeamOs.Tests.Common;

namespace BeamOs.Tests.StructuralAnalysis.Integration;

internal static partial class UnitTestHelpers
{
    public static ISolverFactory SolverFactory { get; set; } = new CholeskySolverFactory();
}

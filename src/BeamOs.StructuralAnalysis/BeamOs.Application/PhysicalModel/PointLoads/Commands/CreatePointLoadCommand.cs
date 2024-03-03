using BeamOs.Application.Common.Commands;
using MathNet.Numerics.LinearAlgebra;
using UnitsNet;

namespace BeamOs.Application.PhysicalModel.PointLoads.Commands;

public record CreatePointLoadCommand(
    GuidBasedIdCommand NodeId,
    Force Force,
    Vector<double> Direction
);

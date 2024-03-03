using BeamOs.Application.Common.Commands;
using MathNet.Numerics.LinearAlgebra;
using UnitsNet;

namespace BeamOs.Application.PhysicalModel.MomentLoads;

public record CreateMomentLoadCommand(
    GuidBasedIdCommand NodeId,
    Torque Torque,
    Vector<double> AxisDirection
);

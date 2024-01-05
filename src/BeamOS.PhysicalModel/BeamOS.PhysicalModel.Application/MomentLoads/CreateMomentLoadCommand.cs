using BeamOS.Common.Application.Commands;
using MathNet.Numerics.LinearAlgebra;
using UnitsNet;

namespace BeamOS.PhysicalModel.Application.MomentLoads;

public record CreateMomentLoadCommand(
    GuidBasedIdCommand NodeId,
    Torque Torque,
    Vector<double> AxisDirection
);

using BeamOS.Common.Application.Commands;
using MathNet.Numerics.LinearAlgebra;
using UnitsNet;

namespace BeamOS.PhysicalModel.Application.PointLoads.Commands;
public record CreatePointLoadCommand(
    GuidBasedIdCommand NodeId,
    Force Force,
    Vector<double> Direction);

using MathNet.Numerics.LinearAlgebra;
using UnitsNet;

namespace BeamOs.Application.DirectStiffnessMethod.MomentLoads;

public record CreateMomentLoadCommand(Torque Torque, Vector<double> AxisDirection);

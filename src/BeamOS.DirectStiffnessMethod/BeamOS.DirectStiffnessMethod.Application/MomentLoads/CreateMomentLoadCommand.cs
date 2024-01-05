using MathNet.Numerics.LinearAlgebra;
using UnitsNet;

namespace BeamOS.DirectStiffnessMethod.Application.MomentLoads;

public record CreateMomentLoadCommand(Torque Torque, Vector<double> AxisDirection);

using MathNet.Numerics.LinearAlgebra;
using UnitsNet;

namespace BeamOS.DirectStiffnessMethod.Application.PointLoads;

public record CreatePointLoadCommand(Force Force, Vector<double> Direction);

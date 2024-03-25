using MathNet.Numerics.LinearAlgebra;
using UnitsNet;

namespace BeamOs.Application.DirectStiffnessMethod.PointLoads;

public record CreatePointLoadCommand(Force Force, Vector<double> Direction);

using BeamOs.Application.Common.Commands;
using MathNet.Spatial.Euclidean;
using UnitsNet;

namespace BeamOs.Application.PhysicalModel.PointLoads.Commands;

public record CreatePointLoadCommand(GuidBasedIdCommand NodeId, Force Force, Vector3D Direction);

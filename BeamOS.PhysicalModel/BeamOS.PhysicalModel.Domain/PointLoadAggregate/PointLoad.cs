using BeamOS.Common.Domain.Models;
using BeamOS.PhysicalModel.Domain.PointLoadAggregate.ValueObjects;
using MathNet.Numerics.LinearAlgebra;
using UnitsNet;

namespace BeamOS.PhysicalModel.Domain.PointLoadAggregate;
public class PointLoad : AggregateRoot<PointLoadId>
{
    public PointLoad(PointLoadId id) : base(id)
    {
    }

    public Force Force { get; set; }
    public Vector<double> NormalizedDirection { get; set; }
}

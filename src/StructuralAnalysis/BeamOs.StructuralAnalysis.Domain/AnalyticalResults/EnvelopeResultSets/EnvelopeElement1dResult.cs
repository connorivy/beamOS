using System.Diagnostics;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.NodeResultAggregate;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using UnitsNet;

namespace BeamOs.StructuralAnalysis.Domain.AnalyticalResults.EnvelopeResultSets;

public sealed class EnvelopeElement1dResult : BeamOsModelEntity<EnvelopeElement1dResultId>
{
    public EnvelopeElement1dResult(
        ModelId modelId,
        Element1dId element1dId,
        ResultSetAndQuantity<Force> maxShear,
        ResultSetAndQuantity<Force> minShear,
        ResultSetAndQuantity<Torque> maxMoment,
        ResultSetAndQuantity<Torque> minMoment,
        ResultSetAndQuantity<Length> maxDisplacement,
        ResultSetAndQuantity<Length> minDisplacement
    )
        : base(new(), modelId)
    {
        this.Element1dId = element1dId;
        this.MaxShear = maxShear;
        this.MinShear = minShear;
        this.MaxMoment = maxMoment;
        this.MinMoment = minMoment;
        this.MaxDisplacement = maxDisplacement;
        this.MinDisplacement = minDisplacement;
    }

    public Element1dId Element1dId { get; set; }
    public ResultSetAndQuantity<Force> MaxShear { get; set; }
    public ResultSetAndQuantity<Force> MinShear { get; set; }
    public ResultSetAndQuantity<Torque> MaxMoment { get; set; }
    public ResultSetAndQuantity<Torque> MinMoment { get; set; }
    public ResultSetAndQuantity<Length> MaxDisplacement { get; set; }
    public ResultSetAndQuantity<Length> MinDisplacement { get; set; }

    public void MergeInResult(Element1dResult element1DResult)
    {
        Trace.Assert(
            this.Element1dId == element1DResult.Element1dId,
            "Element1dId must be the same"
        );
        if (this.MaxShear.Value < element1DResult.MaxShear)
        {
            this.MaxShear = new ResultSetAndQuantity<Force>(
                element1DResult.ResultSetId,
                element1DResult.MaxShear
            );
        }
        if (this.MinShear.Value > element1DResult.MinShear)
        {
            this.MinShear = new ResultSetAndQuantity<Force>(
                element1DResult.ResultSetId,
                element1DResult.MinShear
            );
        }
        if (this.MaxMoment.Value < element1DResult.MaxMoment)
        {
            this.MaxMoment = new ResultSetAndQuantity<Torque>(
                element1DResult.ResultSetId,
                element1DResult.MaxMoment
            );
        }
        if (this.MinMoment.Value > element1DResult.MinMoment)
        {
            this.MinMoment = new ResultSetAndQuantity<Torque>(
                element1DResult.ResultSetId,
                element1DResult.MinMoment
            );
        }
        if (this.MaxDisplacement.Value < element1DResult.MaxDisplacement)
        {
            this.MaxDisplacement = new ResultSetAndQuantity<Length>(
                element1DResult.ResultSetId,
                element1DResult.MaxDisplacement
            );
        }
        if (this.MinDisplacement.Value > element1DResult.MinDisplacement)
        {
            this.MinDisplacement = new ResultSetAndQuantity<Length>(
                element1DResult.ResultSetId,
                element1DResult.MinDisplacement
            );
        }
    }

    public static EnvelopeElement1dResult Create(Element1dResult element1DResult)
    {
        return new(
            element1DResult.ModelId,
            element1DResult.Element1dId,
            new ResultSetAndQuantity<Force>(element1DResult.ResultSetId, element1DResult.MaxShear),
            new ResultSetAndQuantity<Force>(element1DResult.ResultSetId, element1DResult.MinShear),
            new ResultSetAndQuantity<Torque>(
                element1DResult.ResultSetId,
                element1DResult.MaxMoment
            ),
            new ResultSetAndQuantity<Torque>(
                element1DResult.ResultSetId,
                element1DResult.MinMoment
            ),
            new ResultSetAndQuantity<Length>(
                element1DResult.ResultSetId,
                element1DResult.MaxDisplacement
            ),
            new ResultSetAndQuantity<Length>(
                element1DResult.ResultSetId,
                element1DResult.MinDisplacement
            )
        );
    }

    [Obsolete("EF Core Constructor", true)]
    private EnvelopeElement1dResult() { }
}

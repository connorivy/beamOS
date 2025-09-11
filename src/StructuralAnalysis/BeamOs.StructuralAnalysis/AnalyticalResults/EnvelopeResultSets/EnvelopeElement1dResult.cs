using System.Diagnostics;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.NodeResultAggregate;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using UnitsNet;

namespace BeamOs.StructuralAnalysis.Domain.AnalyticalResults.EnvelopeResultSets;

internal sealed class EnvelopeElement1dResult : BeamOsModelEntity<EnvelopeElement1dResultId>
{
    public EnvelopeElement1dResult(
        ModelId modelId,
        EnvelopeResultSetId envelopeResultSetId,
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
        this.EnvelopeResultSetId = envelopeResultSetId;
        this.MaxShear = maxShear;
        this.MinShear = minShear;
        this.MaxMoment = maxMoment;
        this.MinMoment = minMoment;
        this.MaxDisplacement = maxDisplacement;
        this.MinDisplacement = minDisplacement;
    }

    public Element1dId Element1dId { get; set; }
    public EnvelopeResultSetId EnvelopeResultSetId { get; set; }
    public ResultSetAndQuantity<Force> MaxShear { get; set; }
    public ResultSetAndQuantity<Force> MinShear { get; set; }
    public ResultSetAndQuantity<Torque> MaxMoment { get; set; }
    public ResultSetAndQuantity<Torque> MinMoment { get; set; }
    public ResultSetAndQuantity<Length> MaxDisplacement { get; set; }
    public ResultSetAndQuantity<Length> MinDisplacement { get; set; }

    public EnvelopeResultSet? EnvelopeResultSet { get; private set; }

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

    [Obsolete("EF Core Constructor", true)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private EnvelopeElement1dResult() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
}

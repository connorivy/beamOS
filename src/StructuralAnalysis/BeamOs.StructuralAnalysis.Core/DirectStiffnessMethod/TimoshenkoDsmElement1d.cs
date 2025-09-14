using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.Common.Extensions;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.StructuralAnalysis.Domain.DirectStiffnessMethod;

internal class TimoshenkoDsmElement1d(
    Element1dId element1dId,
    Angle sectionProfileRotation,
    Pressure modulusOfElasticity,
    Pressure modulusOfRigidity,
    Area area,
    AreaMomentOfInertia strongAxisMomentOfInertia,
    AreaMomentOfInertia weakAxisMomentOfInertia,
    AreaMomentOfInertia polarMomentOfInertia,
    Area strongAxisShearArea,
    Area weakAxisShearArea,
    Point startPoint,
    Point endPoint,
    NodeId startNodeId,
    NodeId endNodeId
)
    : DsmElement1d(
        element1dId,
        sectionProfileRotation,
        modulusOfElasticity,
        modulusOfRigidity,
        area,
        strongAxisMomentOfInertia,
        weakAxisMomentOfInertia,
        polarMomentOfInertia,
        startPoint,
        endPoint,
        startNodeId,
        endNodeId
    )
{
    // todo: maybe create specific timoshenkoSectionProfile class to not have to throw runtime exceptions
    public TimoshenkoDsmElement1d(Element1d element1d, SectionProfile sectionProfile)
        : this(
            element1d.Id,
            element1d.SectionProfileRotation,
            element1d.Material.ModulusOfElasticity,
            element1d.Material.ModulusOfRigidity,
            sectionProfile.Area,
            sectionProfile.StrongAxisMomentOfInertia,
            sectionProfile.WeakAxisMomentOfInertia,
            sectionProfile.PolarMomentOfInertia,
            sectionProfile.StrongAxisShearArea
                ?? throw new InvalidOperationException(
                    "TimoshenkoDsmElement1d requires strong axis shear area"
                ),
            sectionProfile.WeakAxisShearArea
                ?? throw new InvalidOperationException(
                    "TimoshenkoDsmElement1d requires strong axis shear area"
                ),
            element1d.StartNode.GetLocationPoint(),
            element1d.EndNode.GetLocationPoint(),
            element1d.StartNode.Id,
            element1d.EndNode.Id
        ) { }

    // https://people.duke.edu/~hpgavin/cee421/frame-finite-def.pdf
    public override Matrix<double> GetLocalStiffnessMatrix(
        ForceUnit forceUnit,
        ForcePerLengthUnit forcePerLengthUnit,
        TorqueUnit torqueUnit
    )
    {
#pragma warning disable IDE1006 // Naming Styles
        var E = this.ModulusOfElasticity;
        var G = this.ModulusOfRigidity;
        var A = this.Area;
        var L = this.Length;
        var Is = this.StrongAxisMomentOfInertia;
        var Iw = this.WeakAxisMomentOfInertia;
        var J = this.PolarMomentOfInertia;
        var L2 = L * L;
        var L3 = L2 * L;
        var As = strongAxisShearArea;
        var Asw = weakAxisShearArea;

        // ForcePerLength (N/m, k/in)
        var ExA_L = (E * A / L).As(forcePerLengthUnit);
        var ExIs_L3 = E.MultiplyBy(Is.DivideBy(L3)).As(forcePerLengthUnit);
        var ExIw_L3 = E.MultiplyBy(Iw.DivideBy(L3)).As(forcePerLengthUnit);

        // Force (N, k)
        var ExIs_L2 = E.MultiplyBy(Is.DivideBy(L2)).As(forceUnit);
        var ExIw_L2 = E.MultiplyBy(Iw.DivideBy(L2)).As(forceUnit);

        // Torque (Nm, k-in)
        var ExIs_L = E.MultiplyBy(Is / L).As(torqueUnit);
        var ExIw_L = E.MultiplyBy(Iw / L).As(torqueUnit);
        var GxJ_L = G.MultiplyBy(J / L).As(torqueUnit);

        var PhiW = 12 * E / G * Is / Asw.MultiplyBy(L2);
        var PhiS = 12 * E / G * Iw / As.MultiplyBy(L2);
#pragma warning restore IDE1006 // Naming Styles

        var coeff2_s = (2 - PhiW) / (1 + PhiW);
        var coeff2_w = (2 - PhiS) / (1 + PhiS);

        var coeff6_s = 6 / (1 + PhiW);
        var coeff6_w = 6 / (1 + PhiS);

        var coeff4_s = (4 + PhiW) / (1 + PhiW);
        var coeff4_w = (4 + PhiS) / (1 + PhiS);

        var coeff12_s = 12 / (1 + PhiW);
        var coeff12_w = 12 / (1 + PhiS);

        return DenseMatrix.OfArray(
            new[,]
            {
                { ExA_L, 0, 0, 0, 0, 0, -ExA_L, 0, 0, 0, 0, 0 },
                {
                    0,
                    coeff12_s * ExIs_L3,
                    0,
                    0,
                    0,
                    coeff6_s * ExIs_L2,
                    0,
                    -coeff12_s * ExIs_L3,
                    0,
                    0,
                    0,
                    coeff6_s * ExIs_L2,
                },
                {
                    0,
                    0,
                    coeff12_w * ExIw_L3,
                    0,
                    -coeff6_w * ExIw_L2,
                    0,
                    0,
                    0,
                    -coeff12_w * ExIw_L3,
                    0,
                    -coeff6_w * ExIw_L2,
                    0,
                },
                { 0, 0, 0, GxJ_L, 0, 0, 0, 0, 0, -GxJ_L, 0, 0 },
                {
                    0,
                    0,
                    -coeff6_w * ExIw_L2,
                    0,
                    coeff4_w * ExIw_L,
                    0,
                    0,
                    0,
                    coeff6_w * ExIw_L2,
                    0,
                    coeff2_w * ExIw_L,
                    0,
                },
                {
                    0,
                    coeff6_s * ExIs_L2,
                    0,
                    0,
                    0,
                    coeff4_s * ExIs_L,
                    0,
                    -coeff6_s * ExIs_L2,
                    0,
                    0,
                    0,
                    coeff2_s * ExIs_L,
                },
                { -ExA_L, 0, 0, 0, 0, 0, ExA_L, 0, 0, 0, 0, 0 },
                {
                    0,
                    -coeff12_s * ExIs_L3,
                    0,
                    0,
                    0,
                    -coeff6_s * ExIs_L2,
                    0,
                    coeff12_s * ExIs_L3,
                    0,
                    0,
                    0,
                    -coeff6_s * ExIs_L2,
                },
                {
                    0,
                    0,
                    -coeff12_w * ExIw_L3,
                    0,
                    coeff6_w * ExIw_L2,
                    0,
                    0,
                    0,
                    coeff12_w * ExIw_L3,
                    0,
                    coeff6_w * ExIw_L2,
                    0,
                },
                { 0, 0, 0, -GxJ_L, 0, 0, 0, 0, 0, GxJ_L, 0, 0 },
                {
                    0,
                    0,
                    -coeff6_w * ExIw_L2,
                    0,
                    coeff2_w * ExIw_L,
                    0,
                    0,
                    0,
                    coeff6_w * ExIw_L2,
                    0,
                    coeff4_w * ExIw_L,
                    0,
                },
                {
                    0,
                    coeff6_s * ExIs_L2,
                    0,
                    0,
                    0,
                    coeff2_s * ExIs_L,
                    0,
                    -coeff6_s * ExIs_L2,
                    0,
                    0,
                    0,
                    coeff4_s * ExIs_L,
                },
            }
        );
    }
}

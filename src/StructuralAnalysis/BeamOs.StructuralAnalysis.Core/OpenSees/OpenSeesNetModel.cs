namespace BeamOs.StructuralAnalysis.Domain.OpenSees;

internal class OpenSeesNetModel
{
    public static string ElasticBeamColumn(
        int id,
        int node1Id,
        int node2Id,
        double area,
        double modulusOfElasticity,
        double shearModulus,
        double strongAxisMoment,
        double weakAxisMoment,
        double torsionalMoment,
        int transformId
    //double? mass = null,
    //string? cMass = null,
    //int? releaseCode = null
    ) =>
        //$"element elasticBeamColumn {id} {node1Id} {node2Id} {area} {modulusOfElasticity} {shearModulus} {torsionalMoment} {weakAxisMoment} {strongAxisMoment} {transformId}";

        $"element elasticBeamColumn {id} {node1Id} {node2Id} {area} {modulusOfElasticity} {shearModulus} {torsionalMoment} {strongAxisMoment} {weakAxisMoment} {transformId}";

    public static string ElasticTimoshenkoBeamColumn(
        int id,
        int node1Id,
        int node2Id,
        double area,
        double modulusOfElasticity,
        double shearModulus,
        double strongAxisMoment,
        double weakAxisMoment,
        double torsionalMoment,
        double strongAxisShearArea,
        double weakAxisShearArea,
        int transformId
    //double? mass = null,
    //string? cMass = null,
    //int? releaseCode = null
    ) =>
        $"element ElasticTimoshenkoBeam {id} {node1Id} {node2Id} {modulusOfElasticity} {shearModulus} {area} {torsionalMoment} {strongAxisMoment} {weakAxisMoment} {strongAxisShearArea} {weakAxisShearArea} {transformId}";
}

//internal readonly ref struct ElasticBeamColumnElementParameters
//{
//    public string ElementTag { get; init; }
//    public Span<int> NodeIds { get; init; }
//    public double Area { get; init; }
//    public double ModulusOfElasticity
//}

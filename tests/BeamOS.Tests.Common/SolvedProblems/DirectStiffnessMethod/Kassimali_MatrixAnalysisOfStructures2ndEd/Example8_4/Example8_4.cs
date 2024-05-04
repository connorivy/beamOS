using BeamOs.ApiClient;
using BeamOs.Contracts.Common;
using BeamOs.Contracts.PhysicalModel.Element1d;
using BeamOs.Contracts.PhysicalModel.Material;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOs.Contracts.PhysicalModel.MomentLoad;
using BeamOs.Contracts.PhysicalModel.Node;
using BeamOs.Contracts.PhysicalModel.PointLoad;
using BeamOs.Contracts.PhysicalModel.SectionProfile;

namespace BeamOS.Tests.Common.SolvedProblems.DirectStiffnessMethod.Kassimali_MatrixAnalysisOfStructures2ndEd.Example8_4;

internal partial class Example8_4
{
    public static string ModelId;

    public static ModelResponseHydrated GetExpectedResponse()
    {
        return new(
            ModelId,
            "building name",
            "building description",
            new(UnitSettingsResponse.K_IN),

            [
                new NodeResponse(
                    node1.Id,
                    ModelId,
                    new PointResponse(
                        new UnitValueDto(0, "Inch"),
                        new UnitValueDto(0, "Inch"),
                        new UnitValueDto(0, "Inch")),
                    RestraintResponse.Free
                ),
                new NodeResponse(
                    node2.Id,
                    ModelId,
                    new PointResponse(
                        new UnitValueDto(-240, "Inch"),
                        new UnitValueDto(0, "Inch"),
                        new UnitValueDto(0, "Inch")),
                    RestraintResponse.Fixed
                ),
                new NodeResponse(
                    node3.Id,
                    ModelId,
                    new PointResponse(
                        new UnitValueDto(0, "Inch"),
                        new UnitValueDto(-240, "Inch"),
                        new UnitValueDto(0, "Inch")),
                    RestraintResponse.Fixed
                ),
                new NodeResponse(
                    node4.Id,
                    ModelId,
                    new PointResponse(
                        new UnitValueDto(0, "Inch"),
                        new UnitValueDto(0, "Inch"),
                        new UnitValueDto(-240, "Inch")),
                    RestraintResponse.Fixed
                ),
            ],

            [
                new Element1DResponse(
                    element1.Id,
                    ModelId,
                    node2.Id,
                    node1.Id,
                    steel29000.Id,
                    profile33in2.Id,
                    new UnitValueDto(0, "Radian")
                ),
                new Element1DResponse(
                    element2.Id,
                    ModelId,
                    node3.Id,
                    node1.Id,
                    steel29000.Id,
                    profile33in2.Id,
                    new UnitValueDto(1.5707963267948966, "Radian")
                ),
                new Element1DResponse(
                    element3.Id,
                    ModelId,
                    node4.Id,
                    node1.Id,
                    steel29000.Id,
                    profile33in2.Id,
                    new UnitValueDto(0.5235987755982988, "Radian")
                ),
            ],

            [
                new MaterialResponse(
                    steel29000.Id,
                    new UnitValueDto(29000, "KilopoundForcePerSquareInch"),
                    new UnitValueDto(11500, "KilopoundForcePerSquareInch")
                )
            ],

            [
                new SectionProfileResponse(
                    profile33in2.Id,
                    new UnitValueDto(32.9, "SquareInch"),
                    new UnitValueDto(716, "InchToTheFourth"),
                    new UnitValueDto(236, "InchToTheFourth"),
                    new UnitValueDto(15.1, "InchToTheFourth")
                )
            ],

            [
                new PointLoadResponse(
                    pointLoad1.Id,
                    ModelId,
                    node1.Id,
                    new UnitValueDto(-30, "KilopoundForce"),
                    new Vector3(0, 1, 0)),
                new PointLoadResponse(
                    pointLoad2.Id,
                    ModelId,
                    node2.Id,
                    new UnitValueDto(-30, "KilopoundForce"),
                    new Vector3(0, 1, 0)),
            ],

            [
                new MomentLoadResponse(
                    momentLoad1.Id,
                    node1.Id,
                    new UnitValueDto(-1800, "KilopoundForceInch"),
                    new Vector3(1, 0, 0)),
                new MomentLoadResponse(
                    momentLoad2.Id,
                    node1.Id,
                    new UnitValueDto(1800, "KilopoundForceInch"),
                    new Vector3(0, 0, 1)),
                new MomentLoadResponse(
                    momentLoad3.Id,
                    node1.Id,
                    new UnitValueDto(1200, "KilopoundForceInch"),
                    new Vector3(0, 0, 1)),
                new MomentLoadResponse(
                    momentLoad4.Id,
                    node2.Id,
                    new UnitValueDto(1200, "KilopoundForceInch"),
                    new Vector3(0, 0, -1)),
            ]
        );
    }

    public Example8_4()
    {
        //this.Nodes.Add(Node1);
    }

    //public Example8_4()
    //{
    //    var modelResponse = await client.CreateModelAsync(new("building name", "building description", new ModelSettingsRequest(UnitSettings.K_IN)))
    //}

    public static async Task CreatePhysicalModel(ApiAlphaClient client)
    {
        model ??= await client.CreateModelAsync(
            new(
                "building name",
                "building description",
                new PhysicalModelSettingsDto(UnitSettingsDtoVerbose.K_IN)
            )
        );
        ModelId = model.Id;
        await CreateNodes(client.CreateNodeAsync);
        await CreateMaterials(client.CreateMaterialAsync);
        await CreateSectionProfiles(client.CreateSectionProfileAsync);
        await CreateElement1ds(client.CreateElement1dAsync);
        await CreatePointLoads(client.CreatePointLoadAsync);
        await CreateMomentLoads(client.CreateMomentLoadAsync);
    }

    private static ModelResponse model;

    #region NodeDefinitions
    private static NodeResponse node1;
    private static NodeResponse node2;
    private static NodeResponse node3;
    private static NodeResponse node4;

    public static async Task CreateNodes(Func<CreateNodeRequest, Task<NodeResponse>> clientMethod)
    {
        CreateNodeRequest node1req =
            new(model.Id, 0, 0, 0, "Foot", Restraint: RestraintRequest.Free);
        node1 ??= await clientMethod(node1req);
        //yield return node1req;

        CreateNodeRequest node2req = new(model.Id, -20, 0, 0, "Foot", RestraintRequest.Fixed);
        node2 ??= await clientMethod(node2req);
        //yield return node2req;

        CreateNodeRequest node3req = new(model.Id, 0, -20, 0, "Foot", RestraintRequest.Fixed);
        node3 ??= await clientMethod(node3req);
        //yield return node3req;

        CreateNodeRequest node4req = new(model.Id, 0, 0, -20, "Foot", RestraintRequest.Fixed);
        node4 ??= await clientMethod(node4req);
        //yield return node4req;
    }

    private static MaterialResponse steel29000;

    public static async Task CreateMaterials(
        Func<CreateMaterialRequest, Task<MaterialResponse>> clientMethod
    )
    {
        CreateMaterialRequest steel29000req =
            new(
                model.Id,
                new UnitValueDto(29000, "KilopoundForcePerSquareInch"),
                new(11500, "KilopoundForcePerSquareInch")
            );
        steel29000 ??= await clientMethod(steel29000req);
        //yield return steel29000req;
    }

    private static SectionProfileResponse profile33in2;

    public static async Task CreateSectionProfiles(
        Func<CreateSectionProfileRequest, Task<SectionProfileResponse>> clientMethod
    )
    {
        CreateSectionProfileRequest req =
            new(
                model.Id,
                Area: new(32.9, "SquareInch"),
                StrongAxisMomentOfInertia: new(716, "InchToTheFourth"),
                WeakAxisMomentOfInertia: new(236, "InchToTheFourth"),
                PolarMomentOfInertia: new(15.1, "InchToTheFourth")
            );
        profile33in2 ??= await clientMethod(req);
        //yield return req;
    }

    #endregion

    #region LoadDefinitions
    //private static IEnumerable<PointLoad> GetPointLoads()
    //{
    //    yield return new(
    //        node1.Id,
    //        new Force(-30, ForceUnit.KilopoundForce),
    //        DenseVector.OfArray([0, 1, 0])
    //    );

    //    yield return new(
    //        node2.Id,
    //        new Force(-30, ForceUnit.KilopoundForce),
    //        DenseVector.OfArray([0, 1, 0])
    //    );
    //}

    private static PointLoadResponse pointLoad1;
    private static PointLoadResponse pointLoad2;

    public static async Task CreatePointLoads(
        Func<CreatePointLoadRequest, Task<PointLoadResponse>> clientMethod
    )
    {
        CreatePointLoadRequest req1 =
            new(node1.Id, new(-30, "KilopoundForce"), new Vector3(0, 1, 0));
        pointLoad1 ??= await clientMethod(req1);

        CreatePointLoadRequest req2 =
            new(node2.Id, new(-30, "KilopoundForce"), new Vector3(0, 1, 0));
        pointLoad2 ??= await clientMethod(req2);
    }

    private static MomentLoadResponse momentLoad1;
    private static MomentLoadResponse momentLoad2;
    private static MomentLoadResponse momentLoad3;
    private static MomentLoadResponse momentLoad4;

    public static async Task CreateMomentLoads(
        Func<CreateMomentLoadRequest, Task<MomentLoadResponse>> clientMethod
    )
    {
        CreateMomentLoadRequest req1 =
            new(node1.Id, new(-1800, "KilopoundForceInch"), new Vector3(1, 0, 0));
        momentLoad1 ??= await clientMethod(req1);

        CreateMomentLoadRequest req2 =
            new(node1.Id, new(1800, "KilopoundForceInch"), new Vector3(0, 0, 1));
        momentLoad2 ??= await clientMethod(req2);

        CreateMomentLoadRequest req3 =
            new(node1.Id, new(3 * 20 * 20 / 12, "KilopoundForceFoot"), new Vector3(0, 0, 1));
        momentLoad3 ??= await clientMethod(req3);

        CreateMomentLoadRequest req4 =
            new(node2.Id, new(3 * 20 * 20 / 12, "KilopoundForceFoot"), new Vector3(0, 0, -1));
        momentLoad4 ??= await clientMethod(req4);
    }

    //private static IEnumerable<MomentLoad> GetMomentLoads()
    //{
    //    yield return new MomentLoad(
    //        node1.Id,
    //        new Torque(-1800, TorqueUnit.KilopoundForceInch),
    //        DenseVector.OfArray([1, 0, 0])
    //    );
    //    yield return new MomentLoad(
    //        node1.Id,
    //        new Torque(1800, TorqueUnit.KilopoundForceInch),
    //        DenseVector.OfArray([0, 0, 1])
    //    );

    //    // this moment load represents the fixed end moment of the distributed load.
    //    // this load should be removed when there is support for fixed-end moments.
    //    yield return new MomentLoad(
    //        node1.Id,
    //        new Torque(3 * 20 * 20 / 12, TorqueUnit.KilopoundForceFoot),
    //        DenseVector.OfArray([0, 0, 1])
    //    );

    //    // this moment load represents the fixed end moment of the distributed load.
    //    // this load should be removed when there is support for fixed-end moments.
    //    yield return new MomentLoad(
    //        node2.Id,
    //        new Torque(3 * 20 * 20 / 12, TorqueUnit.KilopoundForceFoot),
    //        DenseVector.OfArray([0, 0, -1])
    //    );
    //}
    #endregion

    #region AnalyticalModelFixtureDefinition

    //public static ModelFixture StaticModelFixture { get; private set; }

    //public static ModelFixture GetAnalyticalModel()
    //{
    //    //var analyticalModel = AnalyticalModel.RunAnalysis(
    //    //    UnitSettings.K_IN,
    //    //    this.Element1dFixtures.Select(f => f.Element),
    //    //    this.Nodes
    //    //);

    //    //var expectedStructureStiffnessMatrix = DenseMatrix.OfArray(
    //    //    new double[,]
    //    //    {
    //    //        { 3990.3, -5.2322, 0, -627.87, -1075.4, 712.92 },
    //    //        { -5.2322, 4008.4, 0, 1800.4, 627.87, -2162.9 },
    //    //        { 0, 0, 3999.4, -2162.9, 712.92, 0 },
    //    //        { -627.87, 1800.4, -2162.9, 634857, 100459, 0 },
    //    //        { -1075.4, 627.87, 712.92, 100459, 286857, 0 },
    //    //        { 712.92, -2162.9, 0, 0, 0, 460857 }
    //    //    }
    //    //);

    //    //var expectedReactionVector = DenseVector.OfArray(

    //    //    [
    //    //        5.3757,
    //    //        44.106 - 30, // subtracting Qf because we're not using fixed end forces. This will change.
    //    //        -0.74272,
    //    //        2.1722,
    //    //        58.987,
    //    //        2330.52 - 1200, // subtracting 1200 for same reason as above ^
    //    //        -4.6249,
    //    //        11.117,
    //    //        -6.4607,
    //    //        -515.55,
    //    //        -0.76472,
    //    //        369.67,
    //    //        -0.75082,
    //    //        4.7763,
    //    //        7.2034,
    //    //        -383.5,
    //    //        -60.166,
    //    //        -4.702
    //    //    ]
    //    //);

    //    //var expectedDisplacementVector =
    //    //    DenseVector.OfArray([-1.3522, -2.7965, -1.812, -3.0021, 1.0569, 6.4986])
    //    //    * Math.Pow(10, -3);

    //    //return new AnalyticalModelFixture(analyticalModel)
    //    //{
    //    //    ExpectedStructureStiffnessMatrix = expectedStructureStiffnessMatrix,
    //    //    NumberOfDecimalsToCompareSMatrix = 0,
    //    //    ExpectedReactionVector = expectedReactionVector,
    //    //    NumberOfDecimalsToCompareReactionVector = 2,
    //    //    ExpectedDisplacementVector = expectedDisplacementVector,
    //    //    NumberOfDecimalsToCompareDisplacementVector = 5
    //    //};

    //    return new ModelFixture(
    //        new Model(
    //            "ModelName",
    //            "ModelDescription",
    //            new ModelSettings(UnitSettings.K_IN),
    //            ModelId
    //        )
    //    );
    //}
    #endregion

    #region Element1dFixtureDefinitions

    private static Element1DResponse element1;
    private static Element1DResponse element2;
    private static Element1DResponse element3;

    public static async Task CreateElement1ds(
        Func<CreateElement1dRequest, Task<Element1DResponse>> clientMethod
    )
    {
        CreateElement1dRequest el1Req =
            new(model.Id, node2.Id, node1.Id, steel29000.Id, profile33in2.Id);
        element1 ??= await clientMethod(el1Req);
        //yield return el1Req;

        CreateElement1dRequest el2Req =
            new(
                model.Id,
                node3.Id,
                node1.Id,
                steel29000.Id,
                profile33in2.Id,
                new(Math.PI / 2, "Radian")
            );

        element2 ??= await clientMethod(el2Req);
        //yield return el2Req;

        CreateElement1dRequest el3Req =
            new(model.Id, node4.Id, node1.Id, steel29000.Id, profile33in2.Id, new(30, "Degree"));
        element3 ??= await clientMethod(el3Req);
        //yield return el3Req;
    }

    //public static Element1dFixture Element1 { get; }
    //public static Element1dFixture Element2 { get; }
    //public static Element1dFixture Element3 { get; }
    //public static SectionProfile Profile33in2 =>
    //    new(
    //        model.Id,
    //        new Area(32.9, AreaUnit.SquareInch),
    //        strongAxisMomentOfInertia: new AreaMomentOfInertia(
    //            716,
    //            AreaMomentOfInertiaUnit.InchToTheFourth
    //        ),
    //        weakAxisMomentOfInertia: new AreaMomentOfInertia(
    //            236,
    //            AreaMomentOfInertiaUnit.InchToTheFourth
    //        ),
    //        polarMomentOfInertia: new AreaMomentOfInertia(
    //            15.1,
    //            AreaMomentOfInertiaUnit.InchToTheFourth
    //        )
    //    );
    //public static Material Steel29000ksi =>
    //    new(
    //        ModelId,
    //        modulusOfElasticity: new Pressure(29000, PressureUnit.KilopoundForcePerSquareInch),
    //        modulusOfRigidity: new Pressure(11500, PressureUnit.KilopoundForcePerSquareInch)
    //    );

    //private static Element1dFixture GetElement1Fixture()
    //{
    //    Element1D element =
    //        new(
    //            ModelId,
    //            node2.Id,
    //            node1.Id,
    //            Steel29000ksi.Id,
    //            Profile33in2.Id,
    //            new(Constants.Guid1)
    //        );

    //    #region ResultsDefinition
    //    var rotationMatrix = DenseMatrix.OfArray(
    //        new double[,]
    //        {
    //            { 1, 0, 0 },
    //            { 0, 1, 0 },
    //            { 0, 0, 1 }
    //        }
    //    );

    //    var transformationMatrix = DenseMatrix.OfArray(
    //        new double[,]
    //        {
    //            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
    //            { 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
    //            { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
    //            { 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0 },
    //            { 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 },
    //            { 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0 },
    //            { 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 },
    //            { 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0 },
    //            { 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0 },
    //            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0 },
    //            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0 },
    //            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 }
    //        }
    //    );

    //    var localStiffnessMatrix = DenseMatrix.OfArray(
    //        new double[,]
    //        {
    //            { 3975.4, 0, 0, 0, 0, 0, -3975.4, 0, 0, 0, 0, 0 },
    //            { 0, 18.024, 0, 0, 0, 2162.9, 0, -18.024, 0, 0, 0, 2162.9 },
    //            { 0, 0, 5.941, 0, -712.92, 0, 0, 0, -5.941, 0, -712.92, 0, },
    //            { 0, 0, 0, 723.54, 0, 0, 0, 0, 0, -723.54, 0, 0 },
    //            { 0, 0, -712.92, 0, 114066.7, 0, 0, 0, 712.92, 0, 57033.3, 0 },
    //            { 0, 2162.9, 0, 0, 0, 346066.7, 0, -2162.9, 0, 0, 0, 173033.3 },
    //            { -3975.4, 0, 0, 0, 0, 0, 3975.4, 0, 0, 0, 0, 0 },
    //            { 0, -18.024, 0, 0, 0, -2162.9, 0, 18.024, 0, 0, 0, -2162.9 },
    //            { 0, 0, -5.941, 0, 712.92, 0, 0, 0, 5.941, 0, 712.92, 0 },
    //            { 0, 0, 0, -723.54, 0, 0, 0, 0, 0, 723.54, 0, 0 },
    //            { 0, 0, -712.92, 0, 57033.3, 0, 0, 0, 712.92, 0, 114066.7, 0 },
    //            { 0, 2162.9, 0, 0, 0, 173033.3, 0, -2162.9, 0, 0, 0, 346066.7 }
    //        }
    //    );

    //    var localFixedEndForces = Vector<double>
    //        .Build
    //        .Dense([0, 30, 0, 0, 0, 12000, 0, 30, 0, 0, 0, -1200]);

    //    var localEndDisplacements =
    //        Vector<double>
    //            .Build
    //            .Dense([0, 0, 0, 0, 0, 0, -1.3522, -2.7965, -1.812, -3.0021, 1.0569, 6.4986])
    //        * Math.Pow(10, -3);

    //    var localEndForces = Vector<double>
    //        .Build
    //        .Dense(

    //            [
    //                5.3757,
    //                44.106,
    //                -0.74272,
    //                2.1722,
    //                58.987,
    //                2330.5,
    //                -5.3757,
    //                15.894,
    //                0.74272,
    //                -2.1722,
    //                119.27,
    //                1055
    //            ]
    //        );
    //    #endregion

    //    return new Element1dFixture(element, UnitSettings.K_IN)
    //    {
    //        ExpectedRotationMatrix = rotationMatrix,
    //        ExpectedTransformationMatrix = transformationMatrix,
    //        ExpectedLocalStiffnessMatrix = localStiffnessMatrix,
    //        ExpectedGlobalStiffnessMatrix = localStiffnessMatrix,
    //        ExpectedLocalFixedEndForces = localFixedEndForces,
    //        ExpectedGlobalFixedEndForces = localFixedEndForces,
    //        ExpectedLocalEndDisplacements = localEndDisplacements,
    //        ExpectedGlobalEndDisplacements = localEndDisplacements,
    //        ExpectedLocalEndForces = localEndForces,
    //        ExpectedGlobalEndForces = localEndForces,
    //    };
    //}

    //public static Element1dFixture GetElement2Fixture()
    //{
    //    Element1D element =
    //        new(
    //            ModelId,
    //            node3.Id,
    //            node1.Id,
    //            Steel29000ksi.Id,
    //            Profile33in2.Id,
    //            new(Constants.Guid2)
    //        )
    //        {
    //            SectionProfileRotation = new Angle(Math.PI / 2, AngleUnit.Radian),
    //        };

    //    #region ResultsDefinition
    //    var rotationMatrix = DenseMatrix.OfArray(
    //        new double[,]
    //        {
    //            { 0, 1, 0 },
    //            { 0, 0, 1 },
    //            { 1, 0, 0 }
    //        }
    //    );

    //    var transformationMatrix = DenseMatrix.OfArray(
    //        new double[,]
    //        {
    //            { 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
    //            { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
    //            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
    //            { 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 },
    //            { 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0 },
    //            { 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0 },
    //            { 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0 },
    //            { 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0 },
    //            { 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 },
    //            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0 },
    //            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
    //            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0 }
    //        }
    //    );

    //    var globalStiffnessMatrix = DenseMatrix.OfArray(
    //        new double[,]
    //        {
    //            { 5.941, 0, 0, 0, 0, -712.92, -5.941, 0, 0, 0, 0, -712.92 },
    //            { 0, 3975.4, 0, 0, 0, 0, 0, -3975.4, 0, 0, 0, 0 },
    //            { 0, 0, 18.024, 2162.9, 0, 0, 0, 0, -18.024, 2162.9, 0, 0 },
    //            { 0, 0, 2162.9, 346066.7, 0, 0, 0, 0, -2162.9, 173033.3, 0, 0 },
    //            { 0, 0, 0, 0, 723.54, 0, 0, 0, 0, 0, -723.54, 0 },
    //            { -712.92, 0, 0, 0, 0, 114066.7, 712.92, 0, 0, 0, 0, 57033.3 },
    //            { -5.941, 0, 0, 0, 0, 712.92, 5.941, 0, 0, 0, 0, 712.92 },
    //            { 0, -3975.4, 0, 0, 0, 0, 0, 3975.4, 0, 0, 0, 0 },
    //            { 0, 0, -18.024, -2162.9, 0, 0, 0, 0, 18.024, -2162.9, 0, 0 },
    //            { 0, 0, 2162.9, 173033.3, 0, 0, 0, 0, -2162.9, 346066.7, 0, 0 },
    //            { 0, 0, 0, 0, -723.54, 0, 0, 0, 0, 0, 723.54, 0 },
    //            { -712.92, 0, 0, 0, 0, 57033.3, 712.92, 0, 0, 0, 0, 114066.7 },
    //        }
    //    );

    //    var localfixedEndForces = Vector<double>
    //        .Build
    //        .Dense(new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });

    //    var localEndDisplacements =
    //        Vector<double>
    //            .Build
    //            .Dense(
    //                new double[]
    //                {
    //                    0,
    //                    0,
    //                    0,
    //                    0,
    //                    0,
    //                    0,
    //                    -2.7965,
    //                    -1.812,
    //                    -1.3522,
    //                    1.0569,
    //                    6.4986,
    //                    -3.0021
    //                }
    //            ) * Math.Pow(10, -3);

    //    var globalEndDisplacements =
    //        Vector<double>
    //            .Build
    //            .Dense(
    //                new double[]
    //                {
    //                    0,
    //                    0,
    //                    0,
    //                    0,
    //                    0,
    //                    0,
    //                    -1.3522,
    //                    -2.7965,
    //                    -1.812,
    //                    -3.0021,
    //                    1.0569,
    //                    6.4986
    //                }
    //            ) * Math.Pow(10, -3);

    //    var localEndForces = Vector<double>
    //        .Build
    //        .Dense(
    //            new double[]
    //            {
    //                11.117,
    //                -6.4607,
    //                -4.6249,
    //                -0.76472,
    //                369.67,
    //                -515.55,
    //                -11.117,
    //                6.4607,
    //                4.6249,
    //                0.76472,
    //                740.31,
    //                -1035,
    //            }
    //        );

    //    var globalEndForces = Vector<double>
    //        .Build
    //        .Dense(
    //            new double[]
    //            {
    //                -4.6249,
    //                11.117,
    //                -6.4607,
    //                -515.55,
    //                -0.76472,
    //                369.67,
    //                4.6249,
    //                -11.117,
    //                6.4607,
    //                -1,
    //                035,
    //                0.76472,
    //                740.31,
    //            }
    //        );
    //    #endregion

    //    return new Element1dFixture(element, UnitSettings.K_IN)
    //    {
    //        ExpectedRotationMatrix = rotationMatrix,
    //        ExpectedTransformationMatrix = transformationMatrix,
    //        //ExpectedLocalStiffnessMatrix = localStiffnessMatrix,
    //        ExpectedGlobalStiffnessMatrix = globalStiffnessMatrix,
    //        ExpectedLocalFixedEndForces = localfixedEndForces,
    //        ExpectedGlobalFixedEndForces = localfixedEndForces,
    //        ExpectedLocalEndDisplacements = localEndDisplacements,
    //        ExpectedGlobalEndDisplacements = localEndDisplacements,
    //        ExpectedLocalEndForces = localEndForces,
    //        ExpectedGlobalEndForces = localEndForces,
    //    };
    //}

    //public static Element1dFixture GetElement3Fixture()
    //{
    //    Element1D element =
    //        new(
    //            ModelId,
    //            node4.Id,
    //            node1.Id,
    //            Steel29000ksi.Id,
    //            Profile33in2.Id,
    //            new(Constants.Guid3)
    //        )
    //        {
    //            SectionProfileRotation = new Angle(30, AngleUnit.Degree),
    //        };

    //    #region ResultsDefinition
    //    var rotationMatrix = DenseMatrix.OfArray(
    //        new double[,]
    //        {
    //            { 0, 0, 1 },
    //            { -.5, .86603, 0 },
    //            { -.86603, -.5, 0 }
    //        }
    //    );

    //    var transformationMatrix = DenseMatrix.OfArray(
    //        new double[,]
    //        {
    //            { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
    //            { -0.5, 0.86603, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
    //            { -0.86603, -0.5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
    //            { 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0 },
    //            { 0, 0, 0, -0.5, 0.86603, 0, 0, 0, 0, 0, 0, 0 },
    //            { 0, 0, 0, -0.86603, -0.5, 0, 0, 0, 0, 0, 0, 0 },
    //            { 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0 },
    //            { 0, 0, 0, 0, 0, 0, -0.5, 0.86603, 0, 0, 0, 0 },
    //            { 0, 0, 0, 0, 0, 0, -0.86603, -0.5, 0, 0, 0, 0 },
    //            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
    //            { 0, 0, 0, 0, 0, 0, 0, 0, 0, -0.5, 0.86603, 0 },
    //            { 0, 0, 0, 0, 0, 0, 0, 0, 0, -0.86603, -0.5, 0 }
    //        }
    //    );

    //    var globalStiffnessMatrix = DenseMatrix.OfArray(
    //        new double[,]
    //        {
    //            { 8.9618, -5.2322, 0, 627.87, 1075.4, 0, -8.9618, 5.2322, 0, 627.87, 1075.4, 0 },
    //            {
    //                -5.2322,
    //                15.003,
    //                0,
    //                -1800.4,
    //                -627.87,
    //                0,
    //                5.2322,
    //                -15.003,
    //                0,
    //                -1800.4,
    //                -627.87,
    //                0
    //            },
    //            { 0, 0, 3975.4, 0, 0, 0, 0, 0, -3975.4, 0, 0, 0 },
    //            {
    //                627.87,
    //                -1800.4,
    //                0,
    //                288066.7,
    //                100458.9,
    //                0,
    //                -627.87,
    //                1800.4,
    //                0,
    //                144033.3,
    //                50229.5,
    //                0
    //            },
    //            {
    //                1075.4,
    //                -627.87,
    //                0,
    //                100458.9,
    //                172066.7,
    //                0,
    //                -1075.4,
    //                627.87,
    //                0,
    //                50229.5,
    //                86033.3,
    //                0
    //            },
    //            { 0, 0, 0, 0, 0, 723.54, 0, 0, 0, 0, 0, -723.54 },
    //            {
    //                -8.9618,
    //                5.2322,
    //                0,
    //                -627.87,
    //                -1075.4,
    //                0,
    //                8.9618,
    //                -5.2322,
    //                0,
    //                -627.87,
    //                -1075.4,
    //                0
    //            },
    //            { 5.2322, -15.003, 0, 1800.4, 627.87, 0, -5.2322, 15.003, 0, 1800.4, 627.87, 0 },
    //            { 0, 0, -3975.4, 0, 0, 0, 0, 0, 3975.4, 0, 0, 0 },
    //            {
    //                627.87,
    //                -1800.4,
    //                0,
    //                144033.3,
    //                50229.5,
    //                0,
    //                -627.87,
    //                1800.4,
    //                0,
    //                288066.7,
    //                100458.9,
    //                0
    //            },
    //            {
    //                1075.4,
    //                -627.87,
    //                0,
    //                50229.5,
    //                86033.3,
    //                0,
    //                -1075.4,
    //                627.87,
    //                0,
    //                100458.9,
    //                172066.7,
    //                0
    //            },
    //            { 0, 0, 0, 0, 0, -723.54, 0, 0, 0, 0, 0, 723.54 }
    //        }
    //    );

    //    var localFixedEndForces = Vector<double>.Build.Dense([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]);

    //    var localEndDisplacements =
    //        Vector<double>
    //            .Build
    //            .Dense([0, 0, 0, 0, 0, 0, -1.812, -1.7457, 2.5693, 6.4986, 2.4164, 2.0714])
    //        * Math.Pow(10, -3);

    //    var globalEndDisplacements =
    //        Vector<double>
    //            .Build
    //            .Dense([0, 0, 0, 0, 0, 0, -1.3522, -2.7965, -1.812, -3.0021, 1.0569, 6.4986])
    //        * Math.Pow(10, -3);

    //    var localEndForces = Vector<double>
    //        .Build
    //        .Dense(

    //            [
    //                7.2034,
    //                4.5118,
    //                -1.7379,
    //                -4.702,
    //                139.65,
    //                362.21,
    //                -7.2034,
    //                -4.5118,
    //                1.7379,
    //                4.702,
    //                277.46,
    //                720.63,
    //            ]
    //        );

    //    var globalEndForces = Vector<double>
    //        .Build
    //        .Dense(

    //            [
    //                -0.75082,
    //                4.7763,
    //                7.2034,
    //                -383.5,
    //                -60.166,
    //                -4.702,
    //                0.75082,
    //                -4.7763,
    //                -7.2034,
    //                -762.82,
    //                -120.03,
    //                4.702,
    //            ]
    //        );
    //    #endregion

    //    return new Element1dFixture(element, UnitSettings.K_IN)
    //    {
    //        ExpectedRotationMatrix = rotationMatrix,
    //        ExpectedTransformationMatrix = transformationMatrix,
    //        //ExpectedLocalStiffnessMatrix = localStiffnessMatrix,
    //        ExpectedGlobalStiffnessMatrix = globalStiffnessMatrix,
    //        ExpectedLocalFixedEndForces = localFixedEndForces,
    //        ExpectedGlobalFixedEndForces = localFixedEndForces,
    //        ExpectedLocalEndDisplacements = localEndDisplacements,
    //        ExpectedGlobalEndDisplacements = localEndDisplacements,
    //        ExpectedLocalEndForces = localEndForces,
    //        ExpectedGlobalEndForces = localEndForces,
    //    };
    //}

    #endregion
}

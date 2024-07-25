using BeamOs.ApiClient.Builders;
using BeamOs.Contracts.Common;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOs.Contracts.PhysicalModel.Node;
using MathNet.Spatial.Euclidean;
using UnitsNet.Units;

namespace BeamOs.Scratchpad.ConsoleApp;

public class CustomModelBuilder : CreateModelRequestBuilder
{
    public string ScratchpadId => "u6ndVnbxFMmYjstMVFFDSA";

    public override Guid ModelGuid { get; } = Guid.Parse("00000000-0000-0000-0000-000000000000");
    public override PhysicalModelSettings ModelSettings { get; } = new(UnitSettingsDtoVerbose.kN_M);

    private int[] xValues = [0, 24];
    private int[] yValues = [0, 12];
    private int[] zValues = [0];

    private CustomModelBuilder()
    {
        //this.CreateMaterialAndSectionProfile();
        //this.CreateNodes();
        //this.CreateVerticalElement1ds();
        //this.CreateHorizontalElement1ds();
        //this.CreatePointLoadsOnRoof();
        //this.CreatePointLoadsOnSide();
    }

    public static async Task<CustomModelBuilder> Create()
    {
        CustomModelBuilder modelBuilder = new();

        await foreach (
            var builder in SpeckleConnector
                .SpeckleConnector
                .ReceiveData("e6b1988124", "f404f297534f6bd4502a42cb1dd08f21")
        )
        {
            if (
                builder is CreateNodeRequestBuilder nodeRequestBuilder
                && nodeRequestBuilder.LocationPoint.YCoordinate.Value > 10000
            )
            {
                modelBuilder.AddPointLoad(
                    new()
                    {
                        NodeId = nodeRequestBuilder.Id,
                        Direction = UnitVector3D.Create(0, -1, 0),
                        Force = new(100, ForceUnit.Kilonewton)
                    }
                );
            }
            modelBuilder.AddElement(builder);
        }

        return modelBuilder;
    }

    private void CreateMaterialAndSectionProfile()
    {
        this.AddMaterial(
            new()
            {
                ModulusOfElasticity = new UnitsNet.Pressure(
                    29000 * .8,
                    PressureUnit.KilopoundForcePerSquareInch
                ),
                ModulusOfRigidity = new UnitsNet.Pressure(
                    11_460,
                    PressureUnit.KilopoundForcePerSquareInch
                )
            }
        );

        this.AddSectionProfile(
            new()
            {
                Area = new UnitsNet.Area(10.6, AreaUnit.SquareInch),
                StrongAxisMomentOfInertia = new UnitsNet.AreaMomentOfInertia(
                    448,
                    AreaMomentOfInertiaUnit.InchToTheFourth
                ),
                WeakAxisMomentOfInertia = new UnitsNet.AreaMomentOfInertia(
                    24.5,
                    AreaMomentOfInertiaUnit.InchToTheFourth
                ),
                PolarMomentOfInertia = new UnitsNet.AreaMomentOfInertia(
                    .55,
                    AreaMomentOfInertiaUnit.InchToTheFourth
                )
            }
        );
    }

    private void CreateNodes()
    {
        for (int xIndex = 0; xIndex < this.xValues.Length; xIndex++)
        {
            for (int yIndex = 0; yIndex < this.yValues.Length; yIndex++)
            {
                for (int zIndex = 0; zIndex < this.zValues.Length; zIndex++)
                {
                    RestraintRequest restraint;
                    if (yIndex > 0)
                    {
                        restraint = RestraintRequest.FreeXzPlane;
                    }
                    else
                    {
                        restraint = RestraintRequest.Fixed;
                    }

                    this.AddNode(
                        new()
                        {
                            LocationPoint = new(
                                this.xValues[xIndex],
                                this.yValues[yIndex],
                                this.zValues[zIndex],
                                "Foot"
                            ),
                            Restraint = restraint,
                            Id = NodeLocationString(
                                this.xValues[xIndex],
                                this.yValues[yIndex],
                                this.zValues[zIndex]
                            )
                        }
                    );
                }
            }
        }
    }

    private void CreateVerticalElement1ds()
    {
        for (int xIndex = 0; xIndex < this.xValues.Length; xIndex++)
        {
            for (int yIndex = 0; yIndex < this.yValues.Length - 1; yIndex++)
            {
                for (int zIndex = 0; zIndex < this.zValues.Length; zIndex++)
                {
                    this.AddElement1d(
                        new()
                        {
                            StartNodeId = NodeLocationString(
                                this.xValues[xIndex],
                                this.yValues[yIndex],
                                this.zValues[zIndex]
                            ),
                            EndNodeId = NodeLocationString(
                                this.xValues[xIndex],
                                this.yValues[yIndex + 1],
                                this.zValues[zIndex]
                            ),
                            SectionProfileRotation = new(90, AngleUnit.Degree)
                        }
                    );
                }
            }
        }
    }

    private void CreateHorizontalElement1ds()
    {
        for (int zIndex = 0; zIndex < this.zValues.Length; zIndex++)
        {
            for (int yIndex = 1; yIndex < this.yValues.Length; yIndex++)
            {
                for (int xIndex = 0; xIndex < this.xValues.Length; xIndex++)
                {
                    if (xIndex != this.xValues.Length - 1)
                    {
                        this.AddElement1d(
                            new()
                            {
                                StartNodeId = NodeLocationString(
                                    this.xValues[xIndex],
                                    this.yValues[yIndex],
                                    this.zValues[zIndex]
                                ),
                                EndNodeId = NodeLocationString(
                                    this.xValues[xIndex + 1],
                                    this.yValues[yIndex],
                                    this.zValues[zIndex]
                                ),
                            }
                        );
                    }

                    if (yIndex != this.yValues.Length - 1)
                    {
                        this.AddElement1d(
                            new()
                            {
                                StartNodeId = NodeLocationString(
                                    this.xValues[xIndex],
                                    this.yValues[yIndex],
                                    this.zValues[zIndex]
                                ),
                                EndNodeId = NodeLocationString(
                                    this.xValues[xIndex],
                                    this.yValues[yIndex],
                                    this.zValues[zIndex + 1]
                                ),
                            }
                        );
                    }
                }
            }
        }
    }

    private void CreatePointLoadsOnRoof()
    {
        int yIndex = this.yValues.Length - 1;
        for (int xIndex = 0; xIndex < this.xValues.Length; xIndex++)
        {
            for (int zIndex = 0; zIndex < this.zValues.Length; zIndex++)
            {
                this.AddPointLoad(
                    new()
                    {
                        NodeId = NodeLocationString(
                            this.xValues[xIndex],
                            this.yValues[yIndex],
                            this.zValues[zIndex]
                        ),
                        Force = new(5, ForceUnit.KilopoundForce),
                        Direction = UnitVector3D.Create(0, -1, 0)
                    }
                );
            }
        }
    }

    private void CreatePointLoadsOnSide()
    {
        int xIndex = this.xValues.Length - 1;
        for (int zIndex = 0; zIndex < this.zValues.Length; zIndex++)
        {
            for (int yIndex = 1; yIndex < this.yValues.Length; yIndex++)
            {
                this.AddPointLoad(
                    new()
                    {
                        NodeId = NodeLocationString(
                            this.xValues[xIndex],
                            this.yValues[yIndex],
                            this.zValues[zIndex]
                        ),
                        Force = new(5, ForceUnit.KilopoundForce),
                        Direction = UnitVector3D.Create(-1, 0, 0)
                    }
                );
            }
        }
    }

    private static string NodeLocationString(int x, int y, int z) => $"n{x} {y} {z}";
}

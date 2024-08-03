using BeamOs.ApiClient.Builders;
using BeamOs.Contracts.Common;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOs.Contracts.PhysicalModel.Node;
using MathNet.Spatial.Euclidean;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.Scratchpad.ConsoleApp;

public class CustomModelBuilder : CreateModelRequestBuilder
{
    public string ScratchpadId => "Hpq0wU_9_wy-NjoGl1h3gA";

    public override Guid ModelGuid { get; } = Guid.Parse("00000000-0000-0000-0000-000000000000");
    public override PhysicalModelSettings ModelSettings { get; } = new(UnitSettingsDtoVerbose.kN_M);

    private int[] xValues = [0, 24, 48, 72];
    private int[] yValues = [0, 12, 24, 36];
    private int[] zValues = [0, 24, 48, 72];

    public CustomModelBuilder()
    {
        this.CreateMaterialAndSectionProfile();
        this.CreateNodes();
        this.CreateVerticalElement1ds();
        this.CreateHorizontalElement1ds();
        this.CreatePointLoadsOnRoof();
        this.CreatePointLoadsOnSide();
    }

    private void CreateMaterialAndSectionProfile()
    {
        this.AddMaterial(
            new()
            {
                ModulusOfElasticity = new Pressure(
                    29000 * .8,
                    PressureUnit.KilopoundForcePerSquareInch
                ),
                ModulusOfRigidity = new Pressure(11_460, PressureUnit.KilopoundForcePerSquareInch)
            }
        );

        this.AddSectionProfile(
            new()
            {
                Area = new Area(10.6, AreaUnit.SquareInch),
                StrongAxisMomentOfInertia = new AreaMomentOfInertia(
                    448,
                    AreaMomentOfInertiaUnit.InchToTheFourth
                ),
                WeakAxisMomentOfInertia = new AreaMomentOfInertia(
                    24.5,
                    AreaMomentOfInertiaUnit.InchToTheFourth
                ),
                PolarMomentOfInertia = new AreaMomentOfInertia(
                    .55,
                    AreaMomentOfInertiaUnit.InchToTheFourth
                ),
                StrongAxisShearArea = new Area(5.0095, AreaUnit.SquareInch),
                WeakAxisShearArea = new Area(4.6905, AreaUnit.SquareInch),
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
                    double xVal = this.xValues[xIndex];
                    double yVal = this.yValues[yIndex];
                    double zVal = this.zValues[zIndex];
                    double zOffset = 0;
                    if (xIndex == 0)
                    {
                        zOffset = -24;
                    }
                    else if (xIndex == this.xValues.Length - 1)
                    {
                        zOffset = 24;
                    }

                    RestraintRequest restraint;
                    if (yIndex > 0)
                    {
                        restraint = RestraintRequest.FreeXzPlane;
                    }
                    else if (xIndex == 0 || zIndex == 0)
                    {
                        restraint = RestraintRequest.Fixed;
                    }
                    else
                    {
                        restraint = RestraintRequest.Pinned;
                    }

                    this.AddNode(
                        new()
                        {
                            LocationPoint = new(
                                this.xValues[xIndex],
                                this.yValues[yIndex],
                                this.zValues[zIndex] + zOffset,
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
                            )
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

                    if (zIndex != this.zValues.Length - 1)
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
                        Force = new(7.5, ForceUnit.KilopoundForce),
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
                        Force = new(3, ForceUnit.KilopoundForce),
                        Direction = UnitVector3D.Create(-1, 0, 0)
                    }
                );
            }
        }
    }

    private static string NodeLocationString(int x, int y, int z) => $"n{x} {y} {z}";
}

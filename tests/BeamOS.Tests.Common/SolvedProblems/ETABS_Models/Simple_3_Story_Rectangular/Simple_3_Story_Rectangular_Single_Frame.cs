using BeamOs.ApiClient.Builders;
using BeamOs.Contracts.Common;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOs.Contracts.PhysicalModel.Node;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using BeamOS.Tests.Common.Fixtures;
using MathNet.Spatial.Euclidean;
using UnitsNet.Units;

namespace BeamOS.Tests.Common.SolvedProblems.ETABS_Models.Simple_3_Story_Rectangular;

public class Simple_3_Story_Rectangular_Single_Frame
    : CreateModelRequestBuilder,
        IHasExpectedNodeDisplacementResults
{
    public override Guid ModelGuid { get; } = Guid.Parse("6f06a260-c88e-43f5-8930-05944a0887ed");
    public override PhysicalModelSettings ModelSettings { get; } = new(UnitSettingsDtoVerbose.K_FT);

    public NodeResultFixture[] ExpectedNodeDisplacementResults { get; } =

        [
            new()
            {
                NodeId = NodeLocationString(0, 0, 0)
            },
            new()
            {
                NodeId = NodeLocationString(0, 12, 0)
            },
            new()
            {
                NodeId = NodeLocationString(24, 12, 0)
            },
            new()
            {
                NodeId = NodeLocationString(24, 0, 0)
            }
        ];

    public ModelSettings Settings { get; } = new(UnitSettings.K_FT);

    private int[] xValues = [0, 24];
    private int[] yValues = [0, 12];
    private int[] zValues = [0];

    private Simple_3_Story_Rectangular_Single_Frame()
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
                ModulusOfElasticity = new UnitsNet.Pressure(
                    29000,
                    PressureUnit.KilopoundForcePerSquareInch
                ),
                ModulusOfRigidity = new UnitsNet.Pressure(
                    11_153.84615,
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
                        restraint = RestraintRequest.FreeXyPlane;
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

    public static Simple_3_Story_Rectangular_Single_Frame Instance { get; } = new();
}

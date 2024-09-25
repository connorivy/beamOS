using BeamOs.ApiClient.Builders;
using BeamOs.Contracts.Common;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOs.Contracts.PhysicalModel.Node;
using BeamOS.Tests.Common.Fixtures;
using MathNet.Spatial.Euclidean;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOS.Tests.Common.SolvedProblems.ETABS_Models.Simple_3_Story_Rectangular;

public partial class Simple_3_Story_Rectangular
    : CreateModelRequestBuilder,
        IHasExpectedNodeDisplacementResults
{
    public override Guid ModelGuid { get; } = Guid.Parse("d19873bf-6909-4da7-91a7-042b9d1a80dd");
    public override PhysicalModelSettings Settings { get; } = new(UnitSettingsContract.K_FT);

    private int[] xValues = [0, 24, 48, 72];
    private int[] yValues = [0, 12, 24, 36];
    private int[] zValues = [0, 24, 48, 72];

    private Simple_3_Story_Rectangular()
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
                    29_000,
                    PressureUnit.KilopoundForcePerSquareInch
                ),
                ModulusOfRigidity = new Pressure(
                    11_153.846,
                    PressureUnit.KilopoundForcePerSquareInch
                )
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
                    RestraintContract restraint;
                    if (yIndex > 0)
                    {
                        restraint = RestraintContract.Free;
                    }
                    else if (zIndex == 0 || xIndex == 0)
                    {
                        restraint = RestraintContract.Fixed;
                    }
                    else
                    {
                        restraint = RestraintContract.Pinned;
                    }

                    this.AddNode(
                        new()
                        {
                            LocationPoint = new(
                                this.xValues[xIndex],
                                this.yValues[yIndex],
                                this.zValues[zIndex],
                                LengthUnitContract.Foot
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

    public static Simple_3_Story_Rectangular Instance { get; } = new();
}

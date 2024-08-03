using BeamOs.ApiClient.Builders;
using BeamOs.Contracts.Common;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOs.Contracts.PhysicalModel.Node;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using MathNet.Spatial.Euclidean;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOS.Tests.Common.SolvedProblems.ETABS_Models.Simple_3_Story_Diagonal;

public partial class Simple_3_Story_Diagonal : CreateModelRequestBuilder
{
    public override Guid ModelGuid { get; } = Guid.Parse("28dc76b9-fa05-4a30-83c9-7c6e938116dd");
    public override PhysicalModelSettings ModelSettings { get; } = new(UnitSettingsDtoVerbose.K_FT);

    public ModelSettings Settings { get; } = new(UnitSettings.K_FT);

    private int[] xValues = [0, 24, 48, 72];
    private int[] yValues = [0, 12, 24, 36];
    private int[] zValues = [0, -24, -48, -72];

    public Simple_3_Story_Diagonal()
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
                ModulusOfElasticity = new Pressure(29000, PressureUnit.KilopoundForcePerSquareInch),
                ModulusOfRigidity = new Pressure(
                    11_153.85,
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

    private int GetZ(int xIndex, int zIndex)
    {
        int zOffset = 0;
        if (xIndex == 0)
        {
            zOffset = -24;
        }
        else if (xIndex == this.xValues.Length - 1)
        {
            zOffset = 24;
        }

        return this.zValues[zIndex] + zOffset;
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
                        restraint = RestraintRequest.Free;
                    }
                    else if (xIndex == 0 || zIndex == this.zValues.Length - 1)
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
                                this.GetZ(xIndex, zIndex),
                                "Foot"
                            ),
                            Restraint = restraint,
                            Id = NodeLocationString(
                                this.xValues[xIndex],
                                this.yValues[yIndex],
                                this.GetZ(xIndex, zIndex)
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
                                this.GetZ(xIndex, zIndex)
                            ),
                            EndNodeId = NodeLocationString(
                                this.xValues[xIndex],
                                this.yValues[yIndex + 1],
                                this.GetZ(xIndex, zIndex)
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
                                    this.GetZ(xIndex, zIndex)
                                ),
                                EndNodeId = NodeLocationString(
                                    this.xValues[xIndex + 1],
                                    this.yValues[yIndex],
                                    this.GetZ(xIndex + 1, zIndex)
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
                                    this.GetZ(xIndex, zIndex)
                                ),
                                EndNodeId = NodeLocationString(
                                    this.xValues[xIndex],
                                    this.yValues[yIndex],
                                    this.GetZ(xIndex, zIndex + 1)
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
                            this.GetZ(xIndex, zIndex)
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
                            this.GetZ(xIndex, zIndex)
                        ),
                        Force = new(3, ForceUnit.KilopoundForce),
                        Direction = UnitVector3D.Create(-1, 0, 0)
                    }
                );
            }
        }
    }

    private static string NodeLocationString(int x, int y, int z) => $"n{x} {y} {z}";

    public static Simple_3_Story_Diagonal Instance { get; } = new();
}

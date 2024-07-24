using BeamOs.ApiClient.Builders;
using BeamOs.Contracts.Common;
using BeamOs.Contracts.PhysicalModel.Node;
using MathNet.Spatial.Euclidean;
using UnitsNet.Units;

namespace BeamOs.Scratchpad.ConsoleApp;

public class ModelResponseFactory
{
    public string ScratchpadId => "g-1IwqUhSEGPGjxPqB5bDg";

    private int[] xValues = [0, 24, 48, 72];
    private int[] yValues = [0, 24, 48, 72];
    private int[] zValues = [0, 12, 24, 36];

    private readonly CreateModelRequestBuilder modelRequestBuilder =
        new() { Settings = new(UnitSettingsDtoVerbose.K_FT) };

    public CreateModelRequestBuilder CreateModelResponse()
    {
        this.CreateNodes();
        this.CreateVerticalElement1ds();
        this.CreateHorizontalElement1ds();
        this.CreatePointLoadsOnRoof();
        this.CreatePointLoadsOnSide();

        return this.modelRequestBuilder;
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
                    if (zIndex > 0)
                    {
                        restraint = RestraintRequest.Free;
                    }
                    else if (xIndex == 0 || yIndex == 0)
                    {
                        restraint = RestraintRequest.Fixed;
                    }
                    else
                    {
                        restraint = RestraintRequest.Pinned;
                    }

                    this.modelRequestBuilder.AddNode(
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
            for (int yIndex = 0; yIndex < this.yValues.Length; yIndex++)
            {
                for (int zIndex = 0; zIndex < this.zValues.Length - 1; zIndex++)
                {
                    this.modelRequestBuilder.AddElement1d(
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

    private void CreateHorizontalElement1ds()
    {
        for (int zIndex = 1; zIndex < this.zValues.Length; zIndex++)
        {
            for (int yIndex = 0; yIndex < this.yValues.Length; yIndex++)
            {
                for (int xIndex = 0; xIndex < this.xValues.Length; xIndex++)
                {
                    if (xIndex != this.xValues.Length - 1)
                    {
                        this.modelRequestBuilder.AddElement1d(
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
                        this.modelRequestBuilder.AddElement1d(
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
                            }
                        );
                    }
                }
            }
        }
    }

    private void CreatePointLoadsOnRoof()
    {
        int zIndex = this.zValues.Length - 1;
        for (int xIndex = 0; xIndex < this.xValues.Length; xIndex++)
        {
            for (int yIndex = 0; yIndex < this.yValues.Length; yIndex++)
            {
                this.modelRequestBuilder.AddPointLoad(
                    new()
                    {
                        NodeId = NodeLocationString(
                            this.xValues[xIndex],
                            this.yValues[yIndex],
                            this.zValues[zIndex]
                        ),
                        Force = new(5, ForceUnit.KilopoundForce),
                        Direction = UnitVector3D.Create(0, 0, -1)
                    }
                );
            }
        }
    }

    private void CreatePointLoadsOnSide()
    {
        int xIndex = this.xValues.Length - 1;
        for (int zIndex = 1; zIndex < this.zValues.Length; zIndex++)
        {
            for (int yIndex = 0; yIndex < this.yValues.Length; yIndex++)
            {
                this.modelRequestBuilder.AddPointLoad(
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

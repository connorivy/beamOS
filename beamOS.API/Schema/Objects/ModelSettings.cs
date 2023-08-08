namespace beamOS.API.Schema.Objects;

using beamOS.API.Schema.Objects.Interfaces;

public class ModelSettings : Base<ModelSettings>, IModelSettings
{
  public double Tolerance { get; } = 1;
  public double MinTreeNodeSize { get; } = 5;
  public int ElementsPerTreeNode { get; } = 10;
  public ModelOrientation ModelOrientation { get; } = ModelOrientation.YUp;
  [Obsolete("For serialization only", true)]
  public ModelSettings() { }
  public ModelSettings(double tolerance, double minTreeNodeSize, int elementsPerTreeNode, ModelOrientation modelOrientation)
  {
    this.Tolerance = tolerance;
    this.MinTreeNodeSize = minTreeNodeSize;
    this.ElementsPerTreeNode = elementsPerTreeNode;
    this.ModelOrientation = modelOrientation;
  }

  public static readonly ModelSettings Dummy = new(1, 5, 10, ModelOrientation.YUp);
}

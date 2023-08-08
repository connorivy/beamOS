namespace beamOS.API.Schema.Objects.Interfaces;

public interface IModelSettings
{
  public double Tolerance { get; }
  public double MinTreeNodeSize { get; }
  public int ElementsPerTreeNode { get; }
  public ModelOrientation ModelOrientation { get; }
}

public enum ModelOrientation
{
  YUp,
  ZUp
}

namespace beamOS.API.Schema.Objects;
using global::Objects.Geometry;

public sealed partial class AnalyticalModel
{
  public OctreeNode OctreeRoot { get; private set; }
  public void ExpandOctree(Point p)
  {
    do
    {
      var offsetDirCenterToPoint = this.OctreeRoot.GetPointOffsetDirections(p);
      var newCenter = new Point(
        this.OctreeRoot.Center.x + (this.OctreeRoot.Size / 2 * (offsetDirCenterToPoint.x == 0 ? 1 : offsetDirCenterToPoint.x)),
        this.OctreeRoot.Center.y + (this.OctreeRoot.Size / 2 * (offsetDirCenterToPoint.y == 0 ? 1 : offsetDirCenterToPoint.y)),
        this.OctreeRoot.Center.z + (this.OctreeRoot.Size / 2 * (offsetDirCenterToPoint.z == 0 ? 1 : offsetDirCenterToPoint.z))
      );
      var newRoot = new OctreeNode(this.modelSettings, newCenter, this.OctreeRoot.Size * 2, this.OctreeRoot);
      this.OctreeRoot = newRoot;
    } while (!this.OctreeRoot.Contains(p));
  }
}

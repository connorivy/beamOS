namespace beamOS.Tests.Schema.Objects;
using System;
using beamOS.API.Schema.Objects;

public class CoordinateDirections<T> : Base<CoordinateDirections<T>>
{
  protected readonly T[] directions = new T[6];
  /// <summary>
  /// Value along the x axis
  /// </summary>
  public T X { get => this.directions[0]; set => this.directions[0] = value; }
  /// <summary>
  /// Value along the y axis
  /// </summary>
  public T Y { get => this.directions[1]; set => this.directions[1] = value; }
  /// <summary>
  /// Value along the z axis
  /// </summary>
  public T Z { get => this.directions[2]; set => this.directions[2] = value; }
  /// <summary>
  /// Value about the x axis
  /// </summary>
  public T AboutX { get => this.directions[3]; set => this.directions[3] = value; }
  /// <summary>
  /// Value about the y axis
  /// </summary>
  public T AboutY { get => this.directions[4]; set => this.directions[4] = value; }
  /// <summary>
  /// Value about the z axis
  /// </summary>
  public T AboutZ { get => this.directions[5]; set => this.directions[5] = value; }
  public CoordinateDirections() { }
  public CoordinateDirections(params T[] dofs)
  {
    if (dofs.Length != 6)
    {
      throw new ArgumentException("Node DOFs must be an array of length 6");
    }
    this.directions = dofs;
  }
}

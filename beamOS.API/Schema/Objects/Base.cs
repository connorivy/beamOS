using Speckle.Core.Models;

namespace beamOS.API.Schema.Objects
{
  public class Base<T> : Base
    where T : Base, new()
  {
  }
}

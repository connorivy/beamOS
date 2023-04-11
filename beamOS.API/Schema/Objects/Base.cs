namespace beamOS.API.Schema.Objects;
using Speckle.Core.Models;

public class Base<T> : Base
  where T : Base, new()
{
}

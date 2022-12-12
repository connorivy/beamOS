using Speckle.Core.Models;

namespace beamOS.API.Schema
{
  public class Query
  {
    public string test => "test me";

    public string GetBase(string commitId)
    {
      return commitId;
    }
  }
}

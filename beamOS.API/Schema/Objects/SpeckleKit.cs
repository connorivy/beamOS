using Speckle.Core.Kits;
using Speckle.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace beamOS.API.Schema.Objects
{
  public class SpeckleKit : ISpeckleKit
  {
    public IEnumerable<Type> Types => Assembly.GetExecutingAssembly().GetTypes()
        .Where(t => t.IsSubclassOf(typeof(Base)) && !t.IsAbstract);

    public IEnumerable<string> Converters => throw new NotImplementedException();

    public string Description => "This is a custom Speckle object kit for beamOS";

    public string Name => "Objects.beamOS";

    public string Author => "Connor Ivy";

    public string WebsiteOrEmail => throw new NotImplementedException();

    public ISpeckleConverter LoadConverter(string app)
    {
      throw new NotImplementedException();
    }
  }
}

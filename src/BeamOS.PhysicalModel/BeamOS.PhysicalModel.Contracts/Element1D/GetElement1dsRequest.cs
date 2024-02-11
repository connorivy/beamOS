using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeamOS.PhysicalModel.Contracts.Element1D;

public record GetElement1dsRequest(string ModelId, [QueryParam] string[]? Element1dIds);

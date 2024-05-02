using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeamOs.Contracts.PhysicalModel.Element1d;

namespace BeamOS.Tests.Common.SolvedProblemFixtures;

public class Element1dFixture(CreateElement1dRequest createElement1DRequest)
{
    public CreateElement1dRequest CreateElement1DRequest { get; } = createElement1DRequest;
}

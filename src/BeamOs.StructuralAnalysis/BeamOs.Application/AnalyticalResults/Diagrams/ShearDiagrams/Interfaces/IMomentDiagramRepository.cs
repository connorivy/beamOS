using BeamOs.Domain.Diagrams.MomentDiagramAggregate;
using BeamOs.Domain.Diagrams.MomentDiagramAggregate.ValueObjects;

namespace BeamOs.Application.AnalyticalResults.Diagrams.ShearDiagrams.Interfaces;

public interface IMomentDiagramRepository : IRepository<MomentDiagramId, MomentDiagram> { }

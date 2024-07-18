using BeamOs.Application.Common.Interfaces.Repositories;
using BeamOs.Domain.Diagrams.ShearForceDiagramAggregate;
using BeamOs.Domain.Diagrams.ShearForceDiagramAggregate.ValueObjects;

namespace BeamOs.Application.AnalyticalResults.Diagrams.ShearDiagrams.Interfaces;

public interface IShearDiagramRepository : IRepository<ShearForceDiagramId, ShearForceDiagram> { }

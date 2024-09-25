using BeamOs.Domain.Diagrams.ShearForceDiagramAggregate;
using BeamOs.Domain.Diagrams.ShearForceDiagramAggregate.ValueObjects;

namespace BeamOs.Application.AnalyticalModel.Diagrams.ShearDiagrams.Interfaces;

public interface IShearDiagramRepository : IRepository<ShearForceDiagramId, ShearForceDiagram> { }

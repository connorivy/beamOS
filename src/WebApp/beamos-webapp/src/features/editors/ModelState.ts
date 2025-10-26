import type {
  Element1dData,
  Element1dResponse,
  InternalNodeData,
  LoadCase,
  LoadCombination,
  MaterialData,
  MaterialResponse,
  MomentLoadResponse,
  ModelResponse,
  ModelSettings,
  NodeData,
  NodeResponse,
  PointLoadResponse,
  SectionProfileData,
  SectionProfileResponse,
  MomentLoadData,
  PointLoadData,
  LoadCaseData,
} from "../../../../../../codeGen/BeamOs.CodeGen.StructuralAnalysisApiClient/StructuralAnalysisApiClientV1"

export type ModelState = {
  id: string
  name: string
  description: string
  settings: ModelSettings
  lastModified: number
  nodes: Record<number, NodeData>
  internalNodes: Record<number, InternalNodeData>
  element1ds: Record<number, Element1dData>
  materials: Record<number, MaterialData>
  sectionProfiles: Record<number, SectionProfileData>
  loadCombinations: Record<number, LoadCombination>
  loadCases: Record<number, LoadCaseData>
  momentLoads: Record<number, MomentLoadData>
  pointLoads: Record<number, PointLoadData>
}

export function NodeResponsesToDataMap(
  nodes: NodeResponse[],
): Record<number, NodeData> {
  const nodeMap: Record<number, NodeData> = {}
  for (const node of nodes) {
    nodeMap[node.id] = {
      locationPoint: node.locationPoint,
      restraint: node.restraint,
    }
  }
  return nodeMap
}

export function Element1dResponsesToDataMap(
  element1ds: Element1dResponse[],
): Record<number, Element1dData> {
  const element1dMap: Record<number, Element1dData> = {}
  for (const element1d of element1ds) {
    element1dMap[element1d.id] = {
      startNodeId: element1d.startNodeId,
      endNodeId: element1d.endNodeId,
      materialId: element1d.materialId,
      sectionProfileId: element1d.sectionProfileId,
      sectionProfileRotation: element1d.sectionProfileRotation,
    }
  }
  return element1dMap
}

export function MaterialResponsesToDataMap(
  materials: MaterialResponse[],
): Record<number, MaterialData> {
  const materialMap: Record<number, MaterialData> = {}
  for (const material of materials) {
    materialMap[material.id] = {
      modulusOfElasticity: material.modulusOfElasticity,
      modulusOfRigidity: material.modulusOfRigidity,
      pressureUnit: material.pressureUnit,
    }
  }
  return materialMap
}

export function sectionProfileResponsesToDataMap(
  sectionProfiles: SectionProfileResponse[],
): Record<number, SectionProfileData> {
  const sectionProfileMap: Record<number, SectionProfileData> = {}
  for (const sectionProfile of sectionProfiles) {
    sectionProfileMap[sectionProfile.id] = {
      area: sectionProfile.area,
      strongAxisMomentOfInertia: sectionProfile.strongAxisMomentOfInertia,
      weakAxisMomentOfInertia: sectionProfile.weakAxisMomentOfInertia,
      polarMomentOfInertia: sectionProfile.polarMomentOfInertia,
      strongAxisPlasticSectionModulus:
        sectionProfile.strongAxisPlasticSectionModulus,
      weakAxisPlasticSectionModulus:
        sectionProfile.weakAxisPlasticSectionModulus,
      strongAxisShearArea: sectionProfile.strongAxisShearArea,
      weakAxisShearArea: sectionProfile.weakAxisShearArea,
      lengthUnit: sectionProfile.lengthUnit,
      name: sectionProfile.name,
    }
  }
  return sectionProfileMap
}

export function LoadCasesToMap(
  loadCases: LoadCase[],
): Record<number, LoadCase> {
  const loadCaseMap: Record<number, LoadCase> = {}
  for (const loadCase of loadCases) {
    loadCaseMap[loadCase.id] = loadCase
  }
  return loadCaseMap
}

export function LoadCombinationsToMap(
  loadCombinations: LoadCombination[],
): Record<number, LoadCombination> {
  const loadCombinationMap: Record<number, LoadCombination> = {}
  for (const loadCombination of loadCombinations) {
    loadCombinationMap[loadCombination.id] = loadCombination
  }
  return loadCombinationMap
}

export function MomentLoadsToMap(
  momentLoads: MomentLoadResponse[],
): Record<number, MomentLoadResponse> {
  const momentLoadMap: Record<number, MomentLoadResponse> = {}
  for (const momentLoad of momentLoads) {
    momentLoadMap[momentLoad.id] = momentLoad
  }
  return momentLoadMap
}

export function PointLoadsToMap(
  pointLoads: PointLoadResponse[],
): Record<number, PointLoadResponse> {
  const pointLoadMap: Record<number, PointLoadResponse> = {}
  for (const pointLoad of pointLoads) {
    pointLoadMap[pointLoad.id] = pointLoad
  }
  return pointLoadMap
}

export function ToModelState(model: ModelResponse): ModelState {
  return {
    id: model.id,
    name: model.name,
    description: model.description,
    settings: model.settings,
    lastModified: new Date(model.lastModified).getDate(),
    nodes: NodeResponsesToDataMap(model.nodes ?? []),
    internalNodes: [], //todo
    element1ds: Element1dResponsesToDataMap(model.element1ds ?? []),
    materials: MaterialResponsesToDataMap(model.materials ?? []),
    sectionProfiles: sectionProfileResponsesToDataMap(
      model.sectionProfiles ?? [],
    ),
    loadCases: LoadCasesToMap(model.loadCases ?? []),
    loadCombinations: LoadCombinationsToMap(model.loadCombinations ?? []),
    momentLoads: MomentLoadsToMap(model.momentLoads ?? []),
    pointLoads: PointLoadsToMap(model.pointLoads ?? []),
  }
}

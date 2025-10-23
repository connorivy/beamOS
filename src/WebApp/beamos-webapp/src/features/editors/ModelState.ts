import type {
  Element1dData,
  Element1dResponse,
  InternalNodeData,
  MaterialData,
  MaterialResponse,
  ModelResponse,
  ModelSettings,
  NodeData,
  NodeResponse,
  SectionProfileData,
  SectionProfileResponse,
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
  }
}

import type {
  ModelResponse,
  ModelSettings,
  NodeData,
  NodeResponse,
} from "../../../../../../codeGen/BeamOs.CodeGen.StructuralAnalysisApiClient/StructuralAnalysisApiClientV1"

export type ModelState = {
  id: string
  name: string
  description: string
  settings: ModelSettings
  lastModified: number
  nodes: Record<number, NodeData>
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

export function ToModelState(model: ModelResponse): ModelState {
  return {
    id: model.id,
    name: model.name,
    description: model.description,
    settings: model.settings,
    lastModified: new Date(model.lastModified).getDate(),
    nodes: NodeResponsesToDataMap(model.nodes ?? []),
  }
}

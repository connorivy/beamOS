import type { NodeResponse } from "../../../../../../../codeGen/BeamOs.CodeGen.EditorApi/EditorApiAlpha"

// eslint-disable-next-line @typescript-eslint/consistent-type-definitions
export interface IEditorEventsApi {
  /**
   * @return OK
   */
  dispatchChangeSelectionCommand(body: ChangeSelectionCommand): Promise<void>

  /**
   * @return OK
   */
  dispatchMoveNodeCommand(body: MoveNodeCommand): Promise<void>

  /**
   * @return OK
   */
  dispatchPutNodeClientCommand(body: PutNodeClientCommand): Promise<void>
}

export class MoveNodeCommand {
  constructor(
    public canvasId: string,
    public modelId: string | undefined,
    public nodeId: number,
    public previousLocation: Coordinate3D,
    public newLocation: Coordinate3D,
  ) {}
}

export class Coordinate3D {
  constructor(
    public x: number,
    public y: number,
    public z: number,
  ) {}
}

export class ChangeSelectionCommand {
  constructor(
    public canvasId: string,
    public selectedObjects: SelectedObject[],
  ) {}
}

export type SelectedObject = {
  id: number
  objectType: number
}

export class PutNodeClientCommand {
  constructor(
    public previous: NodeResponse,
    public newNode: NodeResponse,
    public id: string,
  ) {}
}

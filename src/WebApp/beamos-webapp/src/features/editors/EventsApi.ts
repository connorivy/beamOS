import type {
  ChangeSelectionCommand,
  IEditorEventsApi,
  MoveNodeCommand,
  PutNodeClientCommand,
} from "../three-js-editor/EditorApi/EditorEventsApi"
import { objectSelectionChanged, moveNode } from "./editorsSlice"

export class EventsApi implements IEditorEventsApi {
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  private dispatch: (action: any) => void

  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  constructor(dispatch: (action: any) => void) {
    this.dispatch = dispatch
  }

  dispatchChangeSelectionCommand(body: ChangeSelectionCommand): Promise<void> {
    this.dispatch(
      objectSelectionChanged({
        canvasId: body.canvasId,
        selection: body.selectedObjects,
      }),
    )
    return Promise.resolve()
  }

  dispatchMoveNodeCommand(body: MoveNodeCommand): Promise<void> {
    this.dispatch(
      moveNode({
        command: body,
      }),
    )
    // TODO: Replace with actual moveNode action when available
    // this.dispatch(moveNode({ ...body }))
    return Promise.reject(new Error("MoveNode dispatch not implemented."))
  }

  dispatchPutNodeClientCommand(_body: PutNodeClientCommand): Promise<void> {
    // TODO: Replace with actual putNode action when available
    // this.dispatch(putNode({ ...body }))
    return Promise.reject(new Error("PutNodeClient dispatch not implemented."))
  }
}

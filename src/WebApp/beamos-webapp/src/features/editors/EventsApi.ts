import type {
  ChangeSelectionCommand,
  IEditorEventsApi,
  MoveNodeCommand,
  PutNodeClientCommand,
} from "../three-js-editor/EditorApi/EditorEventsApi"
import { objectSelectionChanged, moveNode } from "./editorsSlice"

export class EventsApi implements IEditorEventsApi {
  private dispatch: (action: unknown) => void

  constructor(dispatch: (action: unknown) => void) {
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

  dispatchPutNodeClientCommand(body: PutNodeClientCommand): Promise<void> {
    // TODO: Replace with actual putNode action when available
    // this.dispatch(putNode({ ...body }))
    return Promise.reject(new Error("PutNodeClient dispatch not implemented."))
  }
}

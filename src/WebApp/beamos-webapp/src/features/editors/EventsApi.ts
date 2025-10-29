import { BeamOsObjectTypes } from "../three-js-editor/EditorApi/EditorApiAlphaExtensions"
import type {
  ChangeSelectionCommand,
  IEditorEventsApi,
  MoveNodeCommand,
  PutNodeClientCommand,
} from "../three-js-editor/EditorApi/EditorEventsApi"
import {
  objectSelectionChanged,
  moveNode,
  setSelectedType,
} from "./editorsSlice"
import { setElement1dId } from "./selection-info/element1d/element1dSelectionSlice"
import { setMomentLoadId } from "./selection-info/momentLoad/momentLoadSelectionSlice"
import { setNodeId } from "./selection-info/node/nodeSelectionSlice"
import { setPointLoadId } from "./selection-info/pointLoad/pointLoadSelectionSlice"

export class EventsApi implements IEditorEventsApi {
  private dispatch: (action: unknown) => void

  constructor(
    private canvasId: string,
    dispatch: (action: unknown) => void,
  ) {
    this.dispatch = dispatch
  }

  dispatchChangeSelectionCommand(body: ChangeSelectionCommand): Promise<void> {
    if (body.selectedObjects.length === 0) {
      this.dispatch(
        setSelectedType({ canvasId: this.canvasId, selectedType: null }),
      )
      this.dispatch(setNodeId(null))
      this.dispatch(setElement1dId(null))
      this.dispatch(setPointLoadId(null))
      this.dispatch(setMomentLoadId(null))
    } else if (
      body.selectedObjects.length === 1 &&
      body.selectedObjects[0].objectType == BeamOsObjectTypes.Node
    ) {
      this.dispatch(
        setSelectedType({
          canvasId: this.canvasId,
          selectedType: BeamOsObjectTypes.Node,
        }),
      )
      this.dispatch(setNodeId(body.selectedObjects[0].id))
      this.dispatch(setElement1dId(null))
      this.dispatch(setPointLoadId(null))
      this.dispatch(setMomentLoadId(null))
    } else if (
      body.selectedObjects.length === 1 &&
      body.selectedObjects[0].objectType == BeamOsObjectTypes.Element1d
    ) {
      this.dispatch(
        setSelectedType({
          canvasId: this.canvasId,
          selectedType: BeamOsObjectTypes.Element1d,
        }),
      )
      this.dispatch(setElement1dId(body.selectedObjects[0].id))
      this.dispatch(setNodeId(null))
      this.dispatch(setPointLoadId(null))
      this.dispatch(setMomentLoadId(null))
    } else if (
      body.selectedObjects.length === 1 &&
      body.selectedObjects[0].objectType == BeamOsObjectTypes.PointLoad
    ) {
      this.dispatch(
        setSelectedType({
          canvasId: this.canvasId,
          selectedType: BeamOsObjectTypes.PointLoad,
        }),
      )
      this.dispatch(setPointLoadId(body.selectedObjects[0].id))
      this.dispatch(setNodeId(null))
      this.dispatch(setElement1dId(null))
      this.dispatch(setMomentLoadId(null))
    } else if (
      body.selectedObjects.length === 1 &&
      body.selectedObjects[0].objectType == BeamOsObjectTypes.MomentLoad
    ) {
      this.dispatch(
        setSelectedType({
          canvasId: this.canvasId,
          selectedType: BeamOsObjectTypes.MomentLoad,
        }),
      )
      this.dispatch(setMomentLoadId(body.selectedObjects[0].id))
      this.dispatch(setNodeId(null))
      this.dispatch(setElement1dId(null))
      this.dispatch(setPointLoadId(null))
    }
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

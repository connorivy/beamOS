import type {
  ChangeSelectionCommand,
  IEditorEventsApi,
  MoveNodeCommand,
  PutNodeClientCommand,
} from "./EditorEventsApi";

type MaybePromise<T> = T | Promise<T>;

export type EditorEventHandlers = {
  onChangeSelection?: (command: ChangeSelectionCommand) => MaybePromise<void>;
  onMoveNode?: (command: MoveNodeCommand) => MaybePromise<void>;
  onPutNodeClient?: (command: PutNodeClientCommand) => MaybePromise<void>;
};

export class ZustandEditorEventsDispatcher implements IEditorEventsApi {
  constructor(private handlers: EditorEventHandlers) {}

  async dispatchChangeSelectionCommand(body: ChangeSelectionCommand): Promise<void> {
    await this.handlers.onChangeSelection?.(body);
  }

  async dispatchMoveNodeCommand(body: MoveNodeCommand): Promise<void> {
    await this.handlers.onMoveNode?.(body);
  }

  async dispatchPutNodeClientCommand(body: PutNodeClientCommand): Promise<void> {
    await this.handlers.onPutNodeClient?.(body);
  }
}


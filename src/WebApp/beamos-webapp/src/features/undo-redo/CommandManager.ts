import { Command, CommandContext } from "./CommandContext"

export class CommandManager {
  private undoStack: Command<any>[] = []
  private redoStack: Command<any>[] = []

  constructor(private readonly context: CommandContext) {}

  async executeCommand<TPayload>(
    command: Command<TPayload>,
    payload: TPayload,
  ): Promise<void> {
    await command.execute(this.context, payload)
    this.undoStack.push(command)
    this.redoStack = [] // Clear redo stack on new command execution
  }

  async undo<TPayload>(payload: TPayload): Promise<void> {
    const command = this.undoStack.pop()
    if (command) {
      await command.undo(this.context, payload)
      this.redoStack.push(command)
    }
  }

  async redo<TPayload>(payload: TPayload): Promise<void> {
    const command = this.redoStack.pop()
    if (command) {
      await command.execute(this.context, payload)
      this.undoStack.push(command)
    }
  }
}

import { IStructuralAnalysisApiClientV1 } from "../../../../../../codeGen/BeamOs.CodeGen.StructuralAnalysisApiClient/StructuralAnalysisApiClientV1";

export interface CommandContext {
    readonly apiClient: IStructuralAnalysisApiClientV1;
}

export  interface Command<TPayload>
{
    execute(context: CommandContext, payload: TPayload): Promise<void>;
    undo(context: CommandContext, payload: TPayload): Promise<void>;
}
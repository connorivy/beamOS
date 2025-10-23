import type { Restraint } from "../../../../../../../codeGen/BeamOs.CodeGen.StructuralAnalysisApiClient/StructuralAnalysisApiClientV1"
import type {
  BeamOsError,
  Result,
} from "../../../../../../../codeGen/BeamOs.CodeGen.EditorApi/EditorApiAlpha"

// eslint-disable-next-line @typescript-eslint/no-extraneous-class
export class BeamOsErrorFactory {
  static None(): BeamOsError {
    return {
      code: "",
      description: "",
      type: ErrorType.None,
      numericType: 0,
      metadata: null,
    }
  }
}

// eslint-disable-next-line @typescript-eslint/no-extraneous-class
export class ResultFactory {
  static Success(): Result {
    return {
      isError: false,
      error: BeamOsErrorFactory.None(),
    }
  }

  static Failure(error: BeamOsError): Result {
    return {
      isError: true,
      error: error,
    }
  }
}

// eslint-disable-next-line @typescript-eslint/no-extraneous-class
export class RestraintContractUtils {
  static GetRestraintType(restraint: Restraint): RestraintType {
    if (
      !restraint.canTranslateAlongX &&
      !restraint.canTranslateAlongY &&
      !restraint.canTranslateAlongZ &&
      restraint.canRotateAboutX &&
      restraint.canRotateAboutY &&
      restraint.canRotateAboutZ
    ) {
      return RestraintType.Pinned
    } else if (
      !restraint.canTranslateAlongX &&
      !restraint.canTranslateAlongY &&
      !restraint.canTranslateAlongZ &&
      !restraint.canRotateAboutX &&
      !restraint.canRotateAboutY &&
      !restraint.canRotateAboutZ
    ) {
      return RestraintType.Fixed
    } else {
      return RestraintType.Other
    }
  }
}

export enum RestraintType {
  Undefined = 0,
  Pinned = 1,
  Fixed = 2,
  Other = 3,
}

export const BeamOsObjectTypes = {
  Undefined: 0,
  Model: 1,
  Node: 2,
  InternalNode: 3,
  Element1d: 4,
  Material: 5,
  SectionProfile: 6,
  SectionProfileFromLibrary: 7,
  PointLoad: 50,
  MomentLoad: 51,
  DistributedLoad: 52,
  DistributedMomentLoad: 53,
  LoadCase: 70,
  LoadCombination: 71,
  ModelProposal: 100,
  NodeProposal: 101,
  InternalNodeProposal: 102,
  Element1dProposal: 103,
  MaterialProposal: 104,
  SectionProfileProposal: 105,
  Other: 255,
}

export const ErrorType = {
  None: 0,
  Failure: 1,
  Validation: 2,
  Conflict: 3,
  NotFound: 4,
  Unauthorized: 5,
  Forbidden: 6,
  InvalidOperation: 7,
}

export function objectTypeToString(beamOsObjectType: number): string {
  switch (beamOsObjectType) {
    case BeamOsObjectTypes.Undefined:
      return "Undefined"
    case BeamOsObjectTypes.Model:
      return "Model"
    case BeamOsObjectTypes.Node:
      return "Node"
    case BeamOsObjectTypes.InternalNode:
      return "InternalNode"
    case BeamOsObjectTypes.Element1d:
      return "Element1d"
    case BeamOsObjectTypes.Material:
      return "Material"
    case BeamOsObjectTypes.SectionProfile:
      return "SectionProfile"
    case BeamOsObjectTypes.SectionProfileFromLibrary:
      return "SectionProfileFromLibrary"
    case BeamOsObjectTypes.PointLoad:
      return "PointLoad"
    case BeamOsObjectTypes.MomentLoad:
      return "MomentLoad"
    case BeamOsObjectTypes.DistributedLoad:
      return "DistributedLoad"

    case BeamOsObjectTypes.DistributedMomentLoad:
      return "DistributedMomentLoad"
    case BeamOsObjectTypes.LoadCase:
      return "LoadCase"
    case BeamOsObjectTypes.LoadCombination:
      return "LoadCombination"
    case BeamOsObjectTypes.ModelProposal:
      return "ModelProposal"
    case BeamOsObjectTypes.NodeProposal:
      return "NodeProposal"
    case BeamOsObjectTypes.InternalNodeProposal:
      return "InternalNodeProposal"
    case BeamOsObjectTypes.Element1dProposal:
      return "Element1dProposal"
    case BeamOsObjectTypes.MaterialProposal:
      return "MaterialProposal"
    case BeamOsObjectTypes.SectionProfileProposal:
      return "SectionProfileProposal"
    case BeamOsObjectTypes.Other:
      return "Other"
    default:
      throw new Error(
        `Unknown BeamOsObjectType: ${beamOsObjectType.toString()}`,
      )
  }
}

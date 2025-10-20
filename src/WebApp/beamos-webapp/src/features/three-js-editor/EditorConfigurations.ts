import { LineMaterial } from "three/examples/jsm/lines/LineMaterial.js"

export class EditorConfigurations {
  public defaultElement1dMaterial: LineMaterial
  public removeNodeProposalHex = 0xf44336 // strong red
  public createNodeProposalHex = 0x4caf50 // medium green
  public modifyNodeProposalHexNew = 0xffeb3b // bright yellow
  public modifyNodeProposalHexExisting = 0xffc107 // amber
  public removeElement1dProposalHex = 0xd32f2f // darker red
  public createElement1dProposalHex = 0x388e3c // darker green
  public modifyElement1dProposalHexNew = 0xfff176 // lighter yellow
  public modifyElement1dProposalHexExisting = 0xffd54f // light amber
  public yAxisUp = false
  public maxShearMagnitude = 0.0001
  public maxMomentMagnitude = 0.0001

  constructor(public isReadOnly: boolean) {
    this.defaultElement1dMaterial = new LineMaterial({
      color: 0x5f8575,
      linewidth: 0.1, // in world units with size attenuation, pixels otherwise
      worldUnits: true,
      vertexColors: false,

      //resolution:  // to be set by renderer, eventually
      alphaToCoverage: true,
    })
  }
}

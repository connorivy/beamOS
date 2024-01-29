import { BeamOsEditor } from 'beamos-editor';

window.createFromId = (editorId) => {
  if (window.beamOsEditor === undefined) {
    window.beamOsEditor = {}
  }
  window.beamOsEditor[editorId] = BeamOsEditor.createFromId(editorId)
  //window.editor.animate()
}

window.interopFunctions = {
  clickElement: function (element) {
    element.click();
  }
}

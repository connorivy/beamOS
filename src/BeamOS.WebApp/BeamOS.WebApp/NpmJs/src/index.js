import { BeamOsEditor } from 'beamos-editor';

window.createFromId = (editorId) => {
  window.beamOsEditor = BeamOsEditor.createFromId(editorId)
  //window.editor.animate()
}

window.interopFunctions = {
  clickElement: function (element) {
    element.click();
  }
}

import { BeamOsEditor } from 'beamos-editor';

// WARNING : the string "createEditorFromId" must match the string in EditorApiProxy.cs
window.createEditorFromId = (editorId, dispatcher) => {
  return BeamOsEditor.createFromId(editorId, dispatcher)
}

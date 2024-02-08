import { BeamOsEditor } from 'beamos-editor';

// WARNING : the string "createEditorFromId" must match the string in EditorApiProxy.cs
window.createEditorFromId = (editorId) => {
  return BeamOsEditor.createFromId(editorId)
}

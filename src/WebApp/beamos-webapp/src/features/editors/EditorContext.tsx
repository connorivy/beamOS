import { createContext, useContext, type ReactNode } from "react"
import type { BeamOsEditor } from "../three-js-editor/BeamOsEditor"

// Create context to share editor references across components
// This avoids storing non-serializable objects in Redux
export const EditorContext = createContext<Record<string, BeamOsEditor | null>>({})

export function EditorProvider({ children, editors }: { children: ReactNode, editors: Record<string, BeamOsEditor | null> }) {
  return (
    <EditorContext.Provider value={editors}>
      {children}
    </EditorContext.Provider>
  )
}

export function useEditor(canvasId: string): BeamOsEditor | null {
  const editors = useContext(EditorContext)
  return editors[canvasId] ?? null
}

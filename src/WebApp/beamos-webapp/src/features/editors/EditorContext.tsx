import { createContext, useContext, type ReactNode } from "react"
import type { BeamOsEditor } from "../three-js-editor/BeamOsEditor"

const EditorContext = createContext<Record<string, BeamOsEditor>>({})

export function EditorProvider({ children }: { children: ReactNode }) {
    return (
        <EditorContext.Provider value={{}}>
            {children}
        </EditorContext.Provider>
    )
}

export function useEditors() {
    const context = useContext(EditorContext)
    return context
}

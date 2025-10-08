import { createContext, useContext, type ReactNode } from "react";
import type { IIdentityApiClientV1 } from "../../../../../../codeGen/BeamOs.CodeGen.StructuralAnalysisApiClient/IdentityApiClientV1";
import { StructuralAnalysisApiClientV1, type IStructuralAnalysisApiClientV1 } from "../../../../../../codeGen/BeamOs.CodeGen.StructuralAnalysisApiClient/StructuralAnalysisApiClientV1";

// Define the context type
export type ApiClientType = IStructuralAnalysisApiClientV1;
export type IdentityApiClientType = IIdentityApiClientV1;

// Create the context with a default (can be undefined for safety)
export const ApiClientContext = createContext<ApiClientType | undefined>(undefined);
export const IdentityApiClientContext = createContext<IdentityApiClientType | undefined>(undefined);

// Provider component
export function ApiClientProvider({ children }: { children: ReactNode }) {
  const backendUrl = import.meta.env.VITE_STRUCTURALBACKEND_URL as string | undefined;
  const apiClient = new StructuralAnalysisApiClientV1(backendUrl);
  return (
    <ApiClientContext.Provider value={apiClient}>
      {children}
    </ApiClientContext.Provider>
  );
}

// Custom hook for consuming the context
export function useApiClient() {
  const context = useContext(ApiClientContext);
  if (!context) {
    throw new Error("useApiClient must be used within an ApiClientProvider");
  }
  return context;
}
export function useIdentityApiClient() {
  const context = useContext(IdentityApiClientContext);
  if (!context) {
    throw new Error("useIdentityApiClient must be used within an IdentityApiClientProvider");
  }
  return context;
}

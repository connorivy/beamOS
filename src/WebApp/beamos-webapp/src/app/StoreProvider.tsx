import { useMemo, type ReactNode } from "react"
import type { ThunkAction, Action } from "@reduxjs/toolkit"
import { Provider } from "react-redux"
import { configureStore } from "@reduxjs/toolkit"
import { useApiClient } from "../features/api-client/ApiClientContext"
import { moveNodeListenerMiddleware } from "../features/api-client/moveNodeListenerMiddleware"
import { counterSlice } from "../features/counter/counterSlice"
import { modelsPageSlice } from "../features/models-page/modelsPageSlice"
import { quotesApiSlice } from "../features/quotes/quotesApiSlice"
import { editorsSlice } from "../features/editors/editorsSlice"
import { combineSlices } from "@reduxjs/toolkit"
import { nodeSelectionSlice } from "../features/editors/selection-info/node/nodeSelectionSlice"

const rootReducer = combineSlices(
  counterSlice,
  quotesApiSlice,
  modelsPageSlice,
  editorsSlice,
  nodeSelectionSlice
)

export type RootState = ReturnType<typeof rootReducer>
export type AppStore = ReturnType<typeof configureStore>
export type AppDispatch = AppStore["dispatch"]
export type AppThunk<ThunkReturnType = void> = ThunkAction<
  ThunkReturnType,
  RootState,
  unknown,
  Action
>

type StoreProviderProps = {
  children: ReactNode
}

export const StoreProvider = ({ children }: StoreProviderProps) => {
  const apiClient = useApiClient()
  const middleware = useMemo(
    () => [
      quotesApiSlice.middleware,
      moveNodeListenerMiddleware(apiClient).middleware,
    ],
    [apiClient],
  )

  const store = useMemo(
    () =>
      configureStore({
        reducer: rootReducer,
        middleware: getDefaultMiddleware =>
          getDefaultMiddleware().concat(middleware),
      }),
    [middleware],
  )

  return <Provider store={store}>{children}</Provider>
}

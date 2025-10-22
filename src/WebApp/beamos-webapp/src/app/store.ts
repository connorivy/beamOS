// import type { Action, ThunkAction } from "@reduxjs/toolkit"
import { combineSlices, configureStore } from "@reduxjs/toolkit"
import { setupListeners } from "@reduxjs/toolkit/query"
import { counterSlice } from "../features/counter/counterSlice"
import { modelsPageSlice } from "../features/models-page/modelsPageSlice"
import { quotesApiSlice } from "../features/quotes/quotesApiSlice"
import { editorsSlice } from "../features/editors/editorsSlice"
import { nodeSelectionSlice } from "../features/editors/selection-info/nodeSelectionSlice"
// import { useApiClient } from "../features/api-client/ApiClientContext"

// Add modelsPageReducer manually to rootReducer
// import { combineReducers } from "@reduxjs/toolkit";
// const rootReducer = combineReducers({
//   counter: counterSlice.reducer,
//   quotesApi: quotesApiSlice.reducer,
//   modelsPage: modelsPageReducer,
// });
const rootReducer = combineSlices(
  counterSlice,
  quotesApiSlice,
  modelsPageSlice,
  editorsSlice,
  nodeSelectionSlice,
)

// Infer the `RootState` type from the root reducer
export type RootState = ReturnType<typeof rootReducer>

// The store setup is wrapped in `makeStore` to allow reuse
// when setting up tests that need the same store config
export const makeStore = (preloadedState?: Partial<RootState>) => {
  const store = configureStore({
    reducer: rootReducer,
    middleware: getDefaultMiddleware =>
      getDefaultMiddleware().concat(quotesApiSlice.middleware),
    preloadedState,
  })
  // configure listeners using the provided defaults
  // optional, but required for `refetchOnFocus`/`refetchOnReconnect` behaviors
  setupListeners(store.dispatch)
  return store
}

export type AppStore = ReturnType<typeof makeStore>

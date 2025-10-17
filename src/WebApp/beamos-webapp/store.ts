import { configureStore } from '@reduxjs/toolkit';
import modelsPageReducer from './src/features/models-page/modelsPageSlice';

const store = configureStore({
  reducer: {
    modelsPage: modelsPageReducer,
  },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
export default store;

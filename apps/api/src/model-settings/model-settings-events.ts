import type { ModelSettingsSnapshot } from "./model-settings-entity";

export type ModelSettingsDomainEvent = {
  type: "model_settings_set";
  payload: ModelSettingsSnapshot;
};

const UUID_REGEX =
  /^[0-9a-f]{8}-[0-9a-f]{4}-[1-8][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i;
const UUID_V7_REGEX =
  /^[0-9a-f]{8}-[0-9a-f]{4}-7[0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i;

export const isUuid = (value: string): boolean => UUID_REGEX.test(value);
export const isUuidV7 = (value: string): boolean => UUID_V7_REGEX.test(value);

export const assertUuid = (value: string, field: string): void => {
  if (!isUuid(value)) {
    throw new Error(`${field} must be a valid UUID`);
  }
};

export const assertUuidV7 = (value: string, field: string): void => {
  if (!isUuidV7(value)) {
    throw new Error(`${field} must be a valid UUIDv7`);
  }
};

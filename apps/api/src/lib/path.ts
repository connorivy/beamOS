export const matchPath = (pattern: string, inputPath: string) => {
  const patternParts = pattern.split("/").filter(Boolean);
  const inputParts = inputPath.split("/").filter(Boolean);

  if (patternParts.length !== inputParts.length) {
    return null;
  }

  const params: Record<string, string> = {};

  for (let i = 0; i < patternParts.length; i += 1) {
    const expected = patternParts[i];
    const actual = inputParts[i];

    if (expected.startsWith(":")) {
      params[expected.slice(1)] = decodeURIComponent(actual);
      continue;
    }

    if (expected !== actual) {
      return null;
    }
  }

  return params;
};

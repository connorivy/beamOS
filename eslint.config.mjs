import js from "@eslint/js";
import tsParser from "@typescript-eslint/parser";
import tsPlugin from "@typescript-eslint/eslint-plugin";

export default [
  js.configs.recommended,
  {
    files: ["**/*.{ts,tsx,mts,cts}"],
    languageOptions: {
      parser: tsParser,
    },
    plugins: {
      "@typescript-eslint": tsPlugin,
    },
    rules: {
      ...tsPlugin.configs.recommended.rules,
      "@typescript-eslint/no-explicit-any": "error",
      "no-undef": "off",
    },
  },
  {
    files: ["apps/api/**/*.{ts,tsx,mts,cts}"],
    languageOptions: {
      globals: {
        Bun: "readonly",
      },
    },
  },
  {
    files: ["apps/web/**/*.{ts,tsx,mts,cts}"],
    rules: {
      "@typescript-eslint/no-explicit-any": "off",
      "@typescript-eslint/no-empty-object-type": "off",
      "@typescript-eslint/no-unused-vars": ["error", { argsIgnorePattern: "^_" }],
      "no-empty": "off",
      "no-redeclare": "off",
      "no-useless-assignment": "off",
    },
  },
];

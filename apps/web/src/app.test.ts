import { describe, expect, it } from "bun:test";
import { getUserResSchema } from "@beamos/contracts";

describe("contracts usable in web", () => {
  it("parses user schema", () => {
    const parsed = getUserResSchema.parse({ id: "1", name: "Ada" });
    expect(parsed.name).toBe("Ada");
  });
});

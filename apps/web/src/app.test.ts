import { describe, expect, it } from "bun:test";
import { getUserResSchema } from "../../api/src/contracts/user";

describe("contracts usable in web", () => {
  it("parses user schema", () => {
    const parsed = getUserResSchema.parse({ id: "1", name: "Ada" });
    expect(parsed.name).toBe("Ada");
  });
});

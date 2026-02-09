import { getUserResSchema } from "@beamos/contracts";

export const fetchUser = async (id: string) => {
  const response = await fetch(`/api/users/${encodeURIComponent(id)}`);
  if (!response.ok) {
    throw new Error(`API error (${response.status})`);
  }

  const data = await response.json();
  return getUserResSchema.parse(data);
};

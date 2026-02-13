export const httpError = (message: string, status: number): Error => {
  const error = new Error(message) as Error & { status: number };
  error.status = status;
  return error;
};

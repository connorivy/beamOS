export const getModelRouteParams = (pathname: string) => {
  const pathSegments = pathname.split("/");
  if (pathSegments.length < 4 || pathSegments[1] !== "models") {
    return null;
  }

  return {
    modelId: decodeURIComponent(pathSegments[2]),
    branchName: decodeURIComponent(pathSegments[3]),
  };
};

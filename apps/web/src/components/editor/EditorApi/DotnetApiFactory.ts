export type DotnetReference = {
    invokeMethodAsync?: (methodName: string, ...args: unknown[]) => unknown;
    [key: string]: unknown;
};

export function DotnetApiFactory<T>(dotnetRef: DotnetReference): T {
    return new Proxy(dotnetRef, {
        get(dotnetReference, prop: string) {
            const invokeMethodName = "invokeMethodAsync";
            const invokeFunc = dotnetReference[invokeMethodName];
            if (typeof invokeFunc === "function") {
                return (...args: unknown[]) => {
                    return invokeFunc.call(
                        dotnetReference,
                        GetCsMethodName(prop),
                        ...args
                    );
                };
            }
        },
    }) as T;
}

function GetCsMethodName(tsName: string) {
    return tsName.charAt(0).toUpperCase() + tsName.slice(1) + "Async";
}

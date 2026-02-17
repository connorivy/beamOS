type DotnetMethodInvoker = {
    invokeMethodAsync: (...args: unknown[]) => unknown;
};

export function DotnetApiFactory<T>(dotnetRef: unknown): T {
    if (typeof dotnetRef !== "object" || dotnetRef === null) {
        throw new Error("dotnetRef must be a non-null object");
    }

    return new Proxy(dotnetRef, {
        get(dotnetReference, prop: string) {
            const invokeMethodName = "invokeMethodAsync";
            const invokeFunc = (dotnetReference as DotnetMethodInvoker)[
                invokeMethodName
            ];
            if (invokeFunc instanceof Function) {
                return function (...args: unknown[]) {
                    return invokeFunc.apply(dotnetReference, [
                        GetCsMethodName(prop),
                        ...args,
                    ]);
                };
            }
        },
    }) as T;
}

function GetCsMethodName(tsName: string) {
    return tsName.charAt(0).toUpperCase() + tsName.slice(1) + "Async";
}

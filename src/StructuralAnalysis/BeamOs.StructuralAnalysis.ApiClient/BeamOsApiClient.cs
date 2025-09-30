using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.StructuralAnalysis.Api;

namespace BeamOs.StructuralAnalysis;

// [DotWrapExpose]
public sealed class BeamOsApiClient : BeamOsFluentApiClient
{
    public BeamOsApiClient(IStructuralAnalysisApiClientV2 apiClient)
        : base(apiClient) { }
}

public sealed class BeamOsResultApiClient : BeamOsFluentResultApiClient, IDisposable
{
    internal List<IDisposable> Disposables { get; } = [];

    public BeamOsResultApiClient(IStructuralAnalysisApiClientV2 apiClient)
        : base(apiClient) { }

    public void Dispose()
    {
        foreach (var disposable in this.Disposables)
        {
            disposable.Dispose();
        }
    }
}

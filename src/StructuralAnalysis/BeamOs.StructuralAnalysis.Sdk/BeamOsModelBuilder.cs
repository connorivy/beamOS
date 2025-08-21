using BeamOs.CodeGen.StructuralAnalysisApiClient;

namespace BeamOs.StructuralAnalysis.Sdk;

public class BeamOsModelBuilder(IBeamOsModel model, IStructuralAnalysisApiClientV1 apiClient)
{
    private static readonly AsyncGuidLockManager LockManager = new();

    /// <summary>
    /// Create the current model, but only if a model doesn't already exist in the current model repository
    /// (could be a local or online repository)
    /// </summary>
    /// <returns>a bool that is true if the model was created or false if it already existed</returns>
    public async Task<bool> CreateOnly() => await this.Build(true);

    /// <summary>
    /// Create the current model if it doesn't exist in the current model repository, or update the
    /// model if it does exist. Only the elements added to the this model builder instance will be updated.
    /// Other existing elements in the model will not be affected.
    /// </summary>
    /// <returns>a bool that is true if the model was created or false if it already existed</returns>
    public async Task<bool> CreateOrUpdate() => await this.Build(false);

    private async Task<bool> Build(bool createOnly)
    {
        if (!Guid.TryParse(model.GuidString, out var modelId))
        {
            throw new Exception("Guid string is not formatted correctly");
        }

        return await LockManager.ExecuteWithLockAsync(
            modelId,
            async () => await this.Build(createOnly, modelId)
        );
    }

    private async Task<bool> Build(bool createOnly, Guid modelId)
    {
        try
        {
            var createModelResult = await apiClient.CreateModelAsync(
                new()
                {
                    Name = model.Name,
                    Description = model.Description,
                    Settings = model.Settings,
                    Id = modelId,
                }
            );
            createModelResult.ThrowIfError();
        }
        catch
        {
            if (createOnly)
            {
                return false;
            }
        }

        foreach (var el in ChunkRequests(model.NodeRequests()))
        {
            (await apiClient.BatchPutNodeAsync(modelId, el)).ThrowIfError();
        }

        foreach (var el in ChunkRequests(model.LoadCaseRequests()))
        {
            (await apiClient.BatchPutLoadCaseAsync(modelId, el)).ThrowIfError();
        }

        foreach (var el in ChunkRequests(model.LoadCombinationRequests()))
        {
            (await apiClient.BatchPutLoadCombinationAsync(modelId, el)).ThrowIfError();
        }

        foreach (var el in ChunkRequests(model.MaterialRequests()))
        {
            (await apiClient.BatchPutMaterialAsync(modelId, el)).ThrowIfError();
        }

        foreach (var el in ChunkRequests(model.SectionProfileRequests()))
        {
            (await apiClient.BatchPutSectionProfileAsync(modelId, el)).ThrowIfError();
        }

        foreach (var el in ChunkRequests(model.SectionProfilesFromLibraryRequests()))
        {
            (await apiClient.BatchPutSectionProfileFromLibraryAsync(modelId, el)).ThrowIfError();
        }

        foreach (var el in ChunkRequests(model.Element1dRequests()))
        {
            (await apiClient.BatchPutElement1dAsync(modelId, el)).ThrowIfError();
        }

        foreach (var el in ChunkRequests(model.InternalNodeRequests()))
        {
            (await apiClient.BatchPutInternalNodeAsync(modelId, el)).ThrowIfError();
        }

        foreach (var el in ChunkRequests(model.PointLoadRequests()))
        {
            (await apiClient.BatchPutPointLoadAsync(modelId, el)).ThrowIfError();
        }

        foreach (var el in ChunkRequests(model.MomentLoadRequests()))
        {
            (await apiClient.BatchPutMomentLoadAsync(modelId, el)).ThrowIfError();
        }

        ModelCreated?.Invoke(this, modelId);

        return true;
    }

    public static event EventHandler<Guid>? ModelCreated;

    private static IEnumerable<List<TRequest>> ChunkRequests<TRequest>(
        IEnumerable<TRequest> requests
    )
    {
        const int batchSize = 50;

        var requestsList = requests.ToList();
        for (int i = 0; i < requestsList.Count; i += batchSize)
        {
            var batch = requestsList.Skip(i).Take(batchSize).ToList();
            yield return batch;
        }
    }
}

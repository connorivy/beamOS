using BeamOs.StructuralAnalysis.Api;

namespace BeamOs.StructuralAnalysis.Sdk;

public class BeamOsModelBuilder(IBeamOsModel model, IStructuralAnalysisApiClientV2 apiClient)
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
        return await LockManager.ExecuteWithLockAsync(
            model.Id,
            async () => await this.Build(createOnly, model.Id)
        );
    }

    private async Task<bool> Build(bool createOnly, Guid modelId)
    {
        try
        {
            var createModelResult = await apiClient.CreateModel(
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
            (await apiClient.BatchPutNode(new() { ModelId = modelId, Body = el })).ThrowIfError();
        }

        foreach (var el in ChunkRequests(model.LoadCaseRequests()))
        {
            (
                await apiClient.BatchPutLoadCase(new() { ModelId = modelId, Body = el })
            ).ThrowIfError();
        }

        foreach (var el in ChunkRequests(model.LoadCombinationRequests()))
        {
            (
                await apiClient.BatchPutLoadCombination(new() { ModelId = modelId, Body = el })
            ).ThrowIfError();
        }

        foreach (var el in ChunkRequests(model.MaterialRequests()))
        {
            (
                await apiClient.BatchPutMaterial(new() { ModelId = modelId, Body = el })
            ).ThrowIfError();
        }

        foreach (var el in ChunkRequests(model.SectionProfileRequests()))
        {
            (
                await apiClient.BatchPutSectionProfile(new() { ModelId = modelId, Body = el })
            ).ThrowIfError();
        }

        foreach (var el in ChunkRequests(model.SectionProfilesFromLibraryRequests()))
        {
            (
                await apiClient.BatchPutSectionProfileFromLibrary(
                    new() { ModelId = modelId, Body = el }
                )
            ).ThrowIfError();
        }

        foreach (var el in ChunkRequests(model.Element1dRequests()))
        {
            (
                await apiClient.BatchPutElement1d(new() { ModelId = modelId, Body = el })
            ).ThrowIfError();
        }

        foreach (var el in ChunkRequests(model.InternalNodeRequests()))
        {
            (
                await apiClient.BatchPutInternalNode(new() { ModelId = modelId, Body = el })
            ).ThrowIfError();
        }

        foreach (var el in ChunkRequests(model.PointLoadRequests()))
        {
            (
                await apiClient.BatchPutPointLoad(new() { ModelId = modelId, Body = el })
            ).ThrowIfError();
        }

        foreach (var el in ChunkRequests(model.MomentLoadRequests()))
        {
            (
                await apiClient.BatchPutMomentLoad(new() { ModelId = modelId, Body = el })
            ).ThrowIfError();
        }

        ModelCreated?.Invoke(this, modelId);

        return true;
    }

    public static event EventHandler<Guid>? ModelCreated;

    private static IEnumerable<TRequest[]> ChunkRequests<TRequest>(IEnumerable<TRequest> requests)
    {
        const int batchSize = 50;

        var requestsList = requests.ToList();
        for (var i = 0; i < requestsList.Count; i += batchSize)
        {
            var batch = requestsList.Skip(i).Take(batchSize).ToArray();
            yield return batch;
        }
    }
}

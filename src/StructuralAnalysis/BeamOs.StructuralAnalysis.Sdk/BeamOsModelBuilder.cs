using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCombinations;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.MomentLoads;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoads;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles;

namespace BeamOs.StructuralAnalysis.Sdk;

public abstract class BeamOsModelBuilder
{
    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract ModelSettings Settings { get; }
    public DateTimeOffset LastModified { get; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// You can go to this website to generate a random guid string
    /// https://www.uuidgenerator.net/guid
    /// </summary>
    public abstract string GuidString { get; }
    public Guid Id => Guid.Parse(this.GuidString);

    public IEnumerable<PutNodeRequest> Nodes => this.NodeRequests();
    public abstract IEnumerable<PutNodeRequest> NodeRequests();
    public IEnumerable<PutMaterialRequest> Materials => this.MaterialRequests();
    public abstract IEnumerable<PutMaterialRequest> MaterialRequests();
    public IEnumerable<PutSectionProfileRequest> SectionProfiles => this.SectionProfileRequests();

    public virtual IEnumerable<PutSectionProfileRequest> SectionProfileRequests() => [];

    public IEnumerable<SectionProfileFromLibrary> SectionProfilesFromLibrary =>
        this.SectionProfilesFromLibraryRequests();

    public virtual IEnumerable<SectionProfileFromLibrary> SectionProfilesFromLibraryRequests() =>
        [];

    public IEnumerable<PutElement1dRequest> Element1ds => this.Element1dRequests();
    public abstract IEnumerable<PutElement1dRequest> Element1dRequests();
    public IEnumerable<PutPointLoadRequest> PointLoads => this.PointLoadRequests();

    public virtual IEnumerable<PutPointLoadRequest> PointLoadRequests() => [];

    public IEnumerable<PutMomentLoadRequest> MomentLoads => this.MomentLoadRequests();

    public virtual IEnumerable<PutMomentLoadRequest> MomentLoadRequests() => [];

    public IEnumerable<LoadCase> LoadCases => this.LoadCaseRequests();

    public abstract IEnumerable<LoadCase> LoadCaseRequests();

    public IEnumerable<LoadCombination> LoadCombinations => this.LoadCombinationRequests();

    public abstract IEnumerable<LoadCombination> LoadCombinationRequests();

    private static AsyncGuidLockManager lockManager = new();

    private async Task<bool> Build(IStructuralAnalysisApiClientV1 apiClient, bool createOnly)
    {
        if (!Guid.TryParse(this.GuidString, out var modelId))
        {
            throw new Exception("Guid string is not formatted correctly");
        }

        return await lockManager.ExecuteWithLockAsync(
            modelId,
            async () =>
            {
                return await this.Build(apiClient, createOnly, modelId);
            }
        );
    }

    private async Task<bool> Build(
        IStructuralAnalysisApiClientV1 apiClient,
        bool createOnly,
        Guid modelId
    )
    {
        try
        {
            var createModelResult = await apiClient.CreateModelAsync(
                new()
                {
                    Name = this.Name,
                    Description = this.Description,
                    Settings = this.Settings,
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

        foreach (var el in ChunkRequests(this.NodeRequests()))
        {
            (await apiClient.BatchPutNodeAsync(modelId, el)).ThrowIfError();
        }

        foreach (var el in ChunkRequests(this.LoadCaseRequests()))
        {
            (await apiClient.BatchPutLoadCaseAsync(modelId, el)).ThrowIfError();
        }

        foreach (var el in ChunkRequests(this.LoadCombinationRequests()))
        {
            (await apiClient.BatchPutLoadCombinationAsync(modelId, el)).ThrowIfError();
        }

        foreach (var el in ChunkRequests(this.PointLoadRequests()))
        {
            (await apiClient.BatchPutPointLoadAsync(modelId, el)).ThrowIfError();
        }

        foreach (var el in ChunkRequests(this.MomentLoadRequests()))
        {
            (await apiClient.BatchPutMomentLoadAsync(modelId, el)).ThrowIfError();
        }

        foreach (var el in ChunkRequests(this.MaterialRequests()))
        {
            (await apiClient.BatchPutMaterialAsync(modelId, el)).ThrowIfError();
        }

        foreach (var el in this.SectionProfileRequests().GroupBy(r => r.GetType()))
        {
            if (el.Key == typeof(PutSectionProfileRequest))
            {
                foreach (
                    var typedProfileRequest in ChunkRequests(el.Cast<PutSectionProfileRequest>())
                )
                {
                    (
                        await apiClient.BatchPutSectionProfileAsync(modelId, typedProfileRequest)
                    ).ThrowIfError();
                }
            }
            // else if (el.Key == typeof(StructuralCodeSectionProfile))
            // {
            //     foreach (
            //         var typedProfileRequest in ChunkRequests(el.Cast<PutSectionProfileRequest>())
            //     )
            //     {
            //         (
            //             await apiClient.BatchPutSectionProfileAsync(modelId, typedProfileRequest)
            //         ).ThrowIfError();
            //     }
            // }
            else
            {
                throw new NotImplementedException($"Section profile type {el.Key} not implemented");
            }
        }

        foreach (var el in ChunkRequests(this.SectionProfilesFromLibraryRequests()))
        {
            (await apiClient.BatchPutSectionProfileFromLibraryAsync(modelId, el)).ThrowIfError();
        }

        foreach (var el in ChunkRequests(this.Element1dRequests()))
        {
            (await apiClient.BatchPutElement1dAsync(modelId, el)).ThrowIfError();
        }

        return true;
    }

    private static IEnumerable<List<TRequest>> ChunkRequests<TRequest>(
        IEnumerable<TRequest> requests
    )
    {
        const int batchSize = 50;

        List<TRequest> requestsList = requests.ToList();
        for (int i = 0; i < requestsList.Count; i += batchSize)
        {
            List<TRequest> batch = requestsList.Skip(i).Take(batchSize).ToList();
            yield return batch;
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="structuralAnalysisApiClient"></param>
    /// <returns>a bool that is true if the model was created or false if it already existed</returns>
    public async Task<bool> CreateOnly(
        IStructuralAnalysisApiClientV1 structuralAnalysisApiClient
    ) => await this.Build(structuralAnalysisApiClient, true);

    /// <summary>
    /// Create or
    /// </summary>
    /// <param name="structuralAnalysisApiClient"></param>
    /// <returns>a bool that is true if the model was created or false if it already existed</returns>
    public async Task<bool> CreateOrUpdate(
        IStructuralAnalysisApiClientV1 structuralAnalysisApiClient
    ) => await this.Build(structuralAnalysisApiClient, false);

    private static StructuralAnalysisApiClientV1 CreateDefaultApiClient()
    {
        HttpClient httpClient = new();
        return new StructuralAnalysisApiClientV1(httpClient);
    }
}

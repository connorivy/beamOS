using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1d;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCombinations;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Material;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Model;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.MomentLoad;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoad;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfile;

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
    public abstract IEnumerable<PutSectionProfileRequest> SectionProfileRequests();
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

    private async Task<bool> Build(IStructuralAnalysisApiClientV1 apiClient, bool createOnly)
    {
        if (!Guid.TryParse(this.GuidString, out var modelId))
        {
            throw new Exception("Guid string is not formatted correctly");
        }

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
            // (await apiClient.BatchPutPointLoadAsync(modelId, el)).ThrowIfError();
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

        foreach (var el in ChunkRequests(this.SectionProfileRequests()))
        {
            (await apiClient.BatchPutSectionProfileAsync(modelId, el)).ThrowIfError();
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

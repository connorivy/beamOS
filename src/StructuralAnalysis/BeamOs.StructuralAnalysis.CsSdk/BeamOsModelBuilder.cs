using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1d;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Material;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Model;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.MomentLoad;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoad;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfile;

namespace BeamOs.StructuralAnalysis.CsSdk;

public abstract class BeamOsModelBuilder
{
    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract PhysicalModelSettings Settings { get; }

    /// <summary>
    /// You can go to this website to generate a random guid string
    /// https://www.uuidgenerator.net/guid
    /// </summary>
    public abstract string GuidString { get; }
    public Guid Id => Guid.Parse(this.GuidString);

    internal IEnumerable<CreateNodeRequest> Nodes => this.NodeRequests();
    public abstract IEnumerable<CreateNodeRequest> NodeRequests();
    internal IEnumerable<CreateMaterialRequest> Materials => this.MaterialRequests();
    public abstract IEnumerable<CreateMaterialRequest> MaterialRequests();
    internal IEnumerable<CreateSectionProfileRequest> SectionProfiles =>
        this.SectionProfileRequests();
    public abstract IEnumerable<CreateSectionProfileRequest> SectionProfileRequests();
    internal IEnumerable<CreateElement1dRequest> Element1ds => this.Element1dRequests();
    public abstract IEnumerable<CreateElement1dRequest> Element1dRequests();
    internal IEnumerable<CreatePointLoadRequest> PointLoads => this.PointLoadRequests();
    public abstract IEnumerable<CreatePointLoadRequest> PointLoadRequests();
    internal IEnumerable<CreateMomentLoadRequest> MomentLoads => this.MomentLoadRequests();

    public virtual IEnumerable<CreateMomentLoadRequest> MomentLoadRequests() => [];

    public Task Build() => this.Build(CreateDefaultApiClient());

    public async Task<bool> Build(
        IStructuralAnalysisApiClientV1 apiClient,
        bool createIfDoesntExist = false
    )
    {
        if (!Guid.TryParse(this.GuidString, out var modelId))
        {
            throw new Exception("Guid string is not formatted correctly");
        }

        try
        {
            await apiClient.CreateModelAsync(
                new()
                {
                    Name = this.Name,
                    Description = this.Description,
                    Settings = this.Settings,
                    Id = modelId
                }
            );
        }
        catch
        {
            if (createIfDoesntExist)
            {
                return false;
            }
            throw;
        }

        // todo : batching
        foreach (var el in this.NodeRequests())
        {
            (await apiClient.CreateNodeAsync(modelId, el)).ThrowIfError();
        }

        foreach (var el in this.PointLoadRequests())
        {
            (await apiClient.CreatePointLoadAsync(modelId, el)).ThrowIfError();
        }

        foreach (var el in this.MomentLoadRequests())
        {
            (await apiClient.CreateMomentLoadAsync(modelId, el)).ThrowIfError();
        }

        foreach (var el in this.MaterialRequests())
        {
            (await apiClient.CreateMaterialAsync(modelId, el)).ThrowIfError();
        }

        foreach (var el in this.SectionProfileRequests())
        {
            (await apiClient.CreateSectionProfileAsync(modelId, el)).ThrowIfError();
        }

        foreach (var el in this.Element1dRequests())
        {
            (await apiClient.CreateElement1dAsync(modelId, el)).ThrowIfError();
        }

        return true;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="structuralAnalysisApiClient"></param>
    /// <returns>a bool that is true if the model was created or false if it already existed</returns>
    public async Task<bool> CreateIfDoesntExist(
        IStructuralAnalysisApiClientV1 structuralAnalysisApiClient
    ) => await this.Build(structuralAnalysisApiClient, true);

    private static StructuralAnalysisApiClientV1 CreateDefaultApiClient()
    {
        HttpClient httpClient = new();
        return new StructuralAnalysisApiClientV1(httpClient);
    }
}

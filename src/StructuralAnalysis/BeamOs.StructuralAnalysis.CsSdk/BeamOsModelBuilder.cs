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
    public abstract IEnumerable<CreateNodeRequest> Nodes();
    public abstract IEnumerable<CreateMaterialRequest> Materials();
    public abstract IEnumerable<CreateSectionProfileRequest> SectionProfiles();
    public abstract IEnumerable<CreateElement1dRequest> Element1ds();
    public abstract IEnumerable<CreatePointLoadRequest> PointLoads();

    public virtual IEnumerable<CreateMomentLoadRequest> MomentLoads() => [];

    public Task Build() => this.Build(CreateDefaultApiClient());

    public async Task Build(IStructuralAnalysisApiClientV1 apiClient)
    {
        if (!Guid.TryParse(this.GuidString, out var modelId))
        {
            throw new Exception("Guid string is not formatted correctly");
        }

        (
            await apiClient.CreateModelAsync(
                new()
                {
                    Name = this.Name,
                    Description = this.Description,
                    Settings = this.Settings,
                    Id = modelId
                }
            )
        ).ThrowIfError();

        // todo : batching
        foreach (var el in this.Nodes())
        {
            (await apiClient.CreateNodeAsync(modelId, el)).ThrowIfError();
        }

        foreach (var el in this.PointLoads())
        {
            (await apiClient.CreatePointLoadAsync(modelId, el)).ThrowIfError();
        }

        foreach (var el in this.MomentLoads())
        {
            (await apiClient.CreateMomentLoadAsync(modelId, el)).ThrowIfError();
        }

        foreach (var el in this.Materials())
        {
            (await apiClient.CreateMaterialAsync(modelId, el)).ThrowIfError();
        }

        foreach (var el in this.SectionProfiles())
        {
            (await apiClient.CreateSectionProfileAsync(modelId, el)).ThrowIfError();
        }

        foreach (var el in this.Element1ds())
        {
            (await apiClient.CreateElement1dAsync(modelId, el)).ThrowIfError();
        }
    }

    private static StructuralAnalysisApiClientV1 CreateDefaultApiClient()
    {
        HttpClient httpClient = new();
        return new StructuralAnalysisApiClientV1(httpClient);
    }
}

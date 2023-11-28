using BeamOS.PhysicalModel.Contracts.Model;

namespace BeamOS.DirectStiffnessMethod.Api;

public class PhysicalModelApiClient(HttpClient httpClient)
{
    public async Task<ModelResponse?> GetModelResponse(string id)
    {
        return await httpClient.GetFromJsonAsync<ModelResponse>($"/api/models/{id}?sendEntities=true");
    }
}

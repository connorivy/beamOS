// See https://aka.ms/new-console-template for more information
using System.Text.Json;
using BeamOs.StructuralAnalysis;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Sdk;

Console.WriteLine("Hello, World!");

BeamOsDynamicModel model = new(
    Guid.NewGuid(),
    new(UnitSettings.K_IN),
    "Test Model",
    "An aot test model"
);

model.AddNode(1, 0.0, 0.0, 0.0);
model.AddNode(2, 120.0, 0.0, 0.0);
model.AddNode(3, 120.0, 0.0, 60.0);
model.AddNode(4, 0.0, 0.0, 60.0);

model.AddMaterial(1, 290000, 1);
model.AddSectionProfileFromLibrary(1, "W12X26", StructuralCode.AISC_360_16);

model.AddElement1d(1, 1, 2, 1, 1);
model.AddElement1d(2, 2, 3, 1, 1);
model.AddElement1d(3, 3, 4, 1, 1);
model.AddElement1d(4, 4, 1, 1, 1);

var client = ApiClientFactory.CreateLocal();
await model.CreateOnly(client);

var el = await client.Models[model.Id].Element1ds[1].GetElement1dAsync();

Console.Write(
    JsonSerializer.Serialize(el, typeof(Element1dResponse), BeamOsJsonSerializerContext.Default)
);
if (el.Id != 1)
{
    throw new Exception("Element id mismatch");
}

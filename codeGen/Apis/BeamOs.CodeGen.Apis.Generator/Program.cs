using BeamOs.CodeGen.Apis.Generator.Apis;
using BeamOs.CodeGen.Apis.Generator.Utils;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder
    .Services
    .AddSwaggerGen(config =>
    {
        config.SchemaFilter<MarkAsRequiredIfNonNullableSchemaProcessor>();
        config.SupportNonNullableReferenceTypes();
        config.SwaggerDoc(
            EditorApiGenerator.EditorApiDocumentName,
            new OpenApiInfo { Title = "Editor Api", Version = "v0" }
        );
        config.SwaggerDoc(
            StructuralAnalysisContractsTypesApiGenerator.StructuralAnalysisContractsApiDocumentName,
            new OpenApiInfo { Title = "Contracts", Version = "v0" }
        );
    });

//builder.Services.AddOpenApi(editorApi);
//builder.Services.AddOpenApi(contracts);

var app = builder.Build();

//app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint(
        $"/swagger/{StructuralAnalysisContractsTypesApiGenerator.StructuralAnalysisContractsApiDocumentName}/swagger.json",
        StructuralAnalysisContractsTypesApiGenerator.StructuralAnalysisContractsApiDocumentName
    );
    options.SwaggerEndpoint(
        $"/swagger/{EditorApiGenerator.EditorApiDocumentName}/swagger.json",
        EditorApiGenerator.EditorApiDocumentName
    );
});

EditorApiGenerator editorApiGenerator = new(app);
StructuralAnalysisContractsTypesApiGenerator contractsTypesApiGenerator = new(app);

//app.Run();

await app.StartAsync();
await editorApiGenerator.GenerateClients();
await contractsTypesApiGenerator.GenerateClients();

using BeamOs.CodeGen.Apis.Generator.Apis;
using BeamOs.CodeGen.Apis.Generator.Utils;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

const string contracts = "contracts";

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
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
app.UseSwaggerUI();

await app.GenerateEditorApi();
await app.GenerateStructuralAnalysisContractsApi();

//app.Run();

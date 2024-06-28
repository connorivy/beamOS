using BeamOs.CodeGen.Apis.Generator.Apis;
using BeamOs.CodeGen.Apis.Generator.Utils;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

/// In order to add a new api generator, just add it to this array. Do not modify the rest of the Program.cs
AbstractGenerator[] generators =
[
    new EditorApiGenerator(),
    new StructuralAnalysisContractsTypesApiGenerator(),
];

/*
 *
 *
 */
builder
    .Services
    .AddSwaggerGen(config =>
    {
        config.SchemaFilter<MarkAsRequiredIfNonNullableSchemaProcessor>();
        config.SupportNonNullableReferenceTypes();

        foreach (AbstractGenerator generator in generators)
        {
            config.SwaggerDoc(
                generator.ClientName,
                new OpenApiInfo { Title = generator.ClientName, Version = "v0" }
            );
        }
    });

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    foreach (AbstractGenerator generator in generators)
    {
        options.SwaggerEndpoint(
            $"/swagger/{generator.ClientName}/swagger.json",
            generator.ClientName
        );
    }
});

foreach (AbstractGenerator generator in generators)
{
    generator.AddMethods(app);
}

//app.Run();

await app.StartAsync();
ILogger logger = app.Services.GetRequiredService<ILogger<Program>>();
foreach (AbstractGenerator generator in generators)
{
    await generator.GenerateClients(logger);
}

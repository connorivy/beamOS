using BeamOs.CodeGen.ApiGenerator.ApiGenerators;

//var builder = WebApplication.CreateBuilder(args);

/// In order to add a new api generator, just add it to this array. Do not modify the rest of the Program.cs
IApiGenerator[] generators =
[
    new EditorApiGenerator(),
    new EditorEventsApi(),
    //new StructuralAnalysisContractsTypesApiGenerator(),
    new StructuralAnalysisApi(),
    new SpeckleConnectorApi(),
    new AiApi(),
];

// foreach (var generator in generators.Where(g => g is not AbstractGenerator))
// {
//     await generator.GenerateClients();
// }

var gen = new StructuralAnalysisApi();
await gen.GenerateClients();

// ///*
// // *
// // *
// // */
// var abstractGenerators = generators.OfType<AbstractGenerator>().ToArray();

// var builder = WebApplication.CreateBuilder(args);

// // builder.Services.AddMvcCore().AddApiExplorer();
// builder.Services.AddSwaggerGen(config =>
// {
//     config.SchemaFilter<MarkAsRequiredIfNonNullableSchemaProcessor>();
//     config.SupportNonNullableReferenceTypes();

//     foreach (AbstractGenerator generator in abstractGenerators)
//     {
//         config.SwaggerDoc(
//             generator.ClientName,
//             new OpenApiInfo { Title = generator.ClientName, Version = "v0" }
//         );
//     }
// });

// var app = builder.Build();

// app.UseSwagger();
// app.UseSwaggerUI(options =>
// {
//     foreach (AbstractGenerator generator in abstractGenerators)
//     {
//         options.SwaggerEndpoint(
//             $"/swagger/{generator.ClientName}/swagger.json",
//             generator.ClientName
//         );
//     }
// });

// foreach (AbstractGenerator generator in abstractGenerators)
// {
//     generator.AddMethods(app);
// }

// await app.StartAsync();
// foreach (AbstractGenerator generator in abstractGenerators)
// {
//     await generator.GenerateClients();
// }

// await app.RunAsync();

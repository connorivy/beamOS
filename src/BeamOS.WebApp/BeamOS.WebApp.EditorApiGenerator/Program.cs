using BeamOs.Api.Common;
using BeamOS.WebApp.EditorApiGenerator;
using FastEndpoints;
using FastEndpoints.Swagger;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

const string alphaRelease = "Alpha Release";
builder
    .Services
    .AddFastEndpoints(options =>
    {
        options.DisableAutoDiscovery = true;
        options.Assemblies =  [typeof(IAssemblyMarkerEditorApiGenerator).Assembly];
    })
    .SwaggerDocument(o =>
    {
        o.DocumentSettings = s =>
        {
            s.DocumentName = alphaRelease;
            s.Title = "beamOS api";
            s.Version = "v0";
            s.SchemaSettings.SchemaProcessors.Add(new MarkAsRequiredIfNonNullableSchemaProcessor());
        };
        o.ShortSchemaNames = true;
        //o.ExcludeNonFastEndpoints = true;
    });

//builder.Services.AddTransient<BeamOsFastEndpointOptions>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseFastEndpoints(c =>
{
    c.Endpoints.RoutePrefix = "api";
    c.Versioning.Prefix = "v";
    c.Endpoints.ShortNames = true;
});

//const string clientNs = "BeamOS.WebApp.EditorApi";
//const string className = "EditorApiAlpha";
//await app.GenerateClientsAndExitAsync(
//    alphaRelease,
//    $"../{clientNs}/",
//    csSettings: c =>
//    {
//        c.ClassName = className;
//        c.GenerateDtoTypes = false;

//        const string beamOsNs = nameof(BeamOS);
//        const string physicalModelNs = nameof(BeamOS.PhysicalModel);
//        const string contractsNs = nameof(BeamOS.PhysicalModel.Contracts);
//        const string nodeNs = nameof(BeamOS.PhysicalModel.Contracts.Node);
//        const string element1dNs = nameof(BeamOS.PhysicalModel.Contracts.Element1D);
//        const string modelNs = nameof(BeamOS.PhysicalModel.Contracts.Model);
//        const string pointLoadNs = nameof(BeamOS.PhysicalModel.Contracts.PointLoad);

//        const string contractsBaseNs = $"{beamOsNs}.{physicalModelNs}.{contractsNs}";
//        c.AdditionalNamespaceUsages =
//        [
//            $"{contractsBaseNs}.{nodeNs}",
//            $"{contractsBaseNs}.{element1dNs}",
//            $"{contractsBaseNs}.{modelNs}",
//            $"{contractsBaseNs}.{pointLoadNs}",
//        ];
//        c.GenerateClientInterfaces = true;
//        c.CSharpGeneratorSettings.Namespace = clientNs;
//        c.UseBaseUrl = false;
//    },
//    tsSettings: t =>
//    {
//        t.ClassName = className;
//        t.GenerateClientInterfaces = true;
//        t.TypeScriptGeneratorSettings.Namespace = ""; // needed to not generate a namespace
//    }
//);

const string clientNs = "BeamOS.WebApp.EditorApi";
const string clientName = "EditorApiAlpha";
const string contractsBaseNs = $"{ApiClientGenerator.BeamOsNs}.{ApiClientGenerator.ContractsNs}";
const string physicalModelBaseNs = $"{contractsBaseNs}.{ApiClientGenerator.PhysicalModelNs}";
const string analyticalResultsBaseNs =
    $"{contractsBaseNs}.{ApiClientGenerator.AnalyticalResultsNs}";

await app.GenerateClient(
    alphaRelease,
    clientNs,
    clientName,

    [
        $"{physicalModelBaseNs}.{ApiClientGenerator.NodeNs}",
        $"{physicalModelBaseNs}.{ApiClientGenerator.Element1dNs}",
        $"{physicalModelBaseNs}.{ApiClientGenerator.ModelNs}",
        $"{physicalModelBaseNs}.{ApiClientGenerator.PointLoadNs}",
        $"{physicalModelBaseNs}.{ApiClientGenerator.MomentLoadNs}",
        $"{physicalModelBaseNs}.{ApiClientGenerator.MaterialNs}",
        $"{physicalModelBaseNs}.{ApiClientGenerator.SectionProfileNs}",
        $"{analyticalResultsBaseNs}.{ApiClientGenerator.AnalyticalModelNs}",
    ]
);

app.Run();

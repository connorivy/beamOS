using BeamOs.Common.Application;
using BeamOs.StructuralAnalysis.SourceGenerator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace BeamOs.Tests.SourceGen;

public static class SnapshotVerifier
{
    public static Task Verify(
        Func<GeneratorDriverRunResult, string> selector,
        params IEnumerable<string> sources
    )
    {
        var attrSource = """
using System;

namespace BeamOs.Common.Api;

public class RouteConstants
{
    public const string ModelRoutePrefixWithTrailingSlash = "models/{modelId:Guid}/";
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class BeamOsRouteAttribute(string value) : Attribute
{
    public string Value { get; } = value;
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class BeamOsEndpointTypeAttribute(string value) : Attribute
{
    public string Value { get; } = value;
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class BeamOsRequiredAuthorizationLevelAttribute(string value) : Attribute
{
    public string Value { get; } = value;
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class BeamOsTagAttribute(string value) : Attribute
{
    public string Value { get; } = value;
}

public static class BeamOsTags
{
    public const string AI = nameof(AI);
}
""";
        // Create references for assemblies we require
        // We could add multiple references if required
        PortableExecutableReference[] references =
        [
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Attribute).Assembly.Location),
            MetadataReference.CreateFromFile(
                typeof(BeamOs.Common.Api.BeamOsRouteAttribute).Assembly.Location
            ),
            MetadataReference.CreateFromFile(typeof(ICommandHandler<,>).Assembly.Location),
            // MetadataReference.CreateFromFile(
            //     typeof(BeamOs.CodeGen.StructuralAnalysisApiClient.IStructuralAnalysisApiClientV1)
            //         .Assembly
            //         .Location
            // ),
        ];

        var compilation = CSharpCompilation.Create(
            "BeamOs.StructuralAnalysis.Generator",
            [
                CSharpSyntaxTree.ParseText(attrSource),
                .. sources.Select(s => CSharpSyntaxTree.ParseText(s)),
            ],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        // var generator = new BeamOsRouteInMemoryGenerator().AsSourceGenerator();
        // GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        // driver = driver.RunGeneratorsAndUpdateCompilation(
        //     compilation,
        //     out var outputCompilation,
        //     out var diagnostics
        // );
        // var result = driver.GetRunResult();

        // return Verifier.Verify(selector(result));
        return Task.CompletedTask;
    }
}

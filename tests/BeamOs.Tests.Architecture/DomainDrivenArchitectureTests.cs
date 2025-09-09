using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.Loader;
using BeamOs.Common.Api;
using BeamOs.Common.Application;
using BeamOs.Common.Domain.Models;
using BeamOs.StructuralAnalysis.Api.Endpoints;
using BeamOs.StructuralAnalysis.Application;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Infrastructure;
using Microsoft.EntityFrameworkCore;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace BeamOs.Tests.Architecture;

public class DomainDrivenArchitectureTests
{
    private static readonly ArchUnitNET.Domain.Architecture Architecture = new ArchLoader()
        .LoadAssemblies(
            typeof(IAssemblyMarkerStructuralAnalysisApplication).Assembly,
            typeof(IAssemblyMarkerInfrastructure).Assembly,
            typeof(IAssemblyMarkerCommonApplication).Assembly,
            typeof(IAssemblyMarkerStructuralAnalysisApplication).Assembly,
            typeof(IAssemblyMarkerStructuralAnalysisApiEndpoints).Assembly
        )
        .Build();

    private readonly IObjectProvider<Class> Endpoints = Classes()
        .That()
        .AreAssignableTo(typeof(BeamOsBaseEndpoint<,>))
        .And()
        .AreNotAbstract()
        .As("Endpoints");

    private readonly IObjectProvider<IType> DomainTypes = Types()
        .That()
        .ImplementInterface(typeof(IBeamOsDomainObject))
        .As("Domain");

    private readonly IObjectProvider<IType> Repositories = Types()
        .That()
        .ImplementInterface(typeof(IRepository<,>))
        .As("Repositories");

    private readonly IObjectProvider<IType> CommandHandler = Types()
        .That()
        .ImplementInterface(typeof(ICommandHandler<,>))
        .As("CommandHandlers");

    private readonly IObjectProvider<IType> QueryHandlers = Types()
        .That()
        .ImplementInterface(typeof(IQueryHandler<,>))
        .As("QueryHandlers");

    [Test]
    public void Endpoints_ShouldNotReferenceRepositories()
    {
        IArchRule rule = Classes()
            .That()
            .Are(this.Endpoints)
            .Should()
            .NotDependOnAny(this.Repositories);

        rule.Check(Architecture);
    }

    /// <summary>
    /// This test ensures that endpoints depend on command handlers or query handlers.
    /// This is a good practice to ensure that the endpoints are not doing too much work.
    /// </summary>
    [Test]
    public void Endpoints_ShouldDependOnCommandHandlers_OrQueryHandlers()
    {
        IArchRule rule = Types()
            .That()
            .Are(this.Endpoints)
            .Should()
            .DependOnAny(this.CommandHandler)
            .OrShould()
            .DependOnAny(typeof(ICommandHandler<,>))
            .OrShould()
            .DependOnAny(this.QueryHandlers)
            .OrShould()
            .DependOnAny(typeof(IQueryHandler<,>));

        rule.Check(Architecture);
    }

    [Test]
    public void CommandHandlers_ShouldUseRepository_NotDbContext()
    {
        IArchRule rule = Types()
            .That()
            .Are(this.CommandHandler)
            .Should()
            .NotDependOnAnyTypesThat()
            .AreAssignableTo(typeof(DbContext));

        rule.Check(Architecture);

        // rule = Types().That().Are(this.CommandHandler).Should().DependOnAny(this.Repositories);

        // rule.Check(Architecture);
    }
}

using BeamOs.StructuralAnalysis.Api.Endpoints.OpenSees;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Models.Mappers;
using BeamOs.StructuralAnalysis.Domain.DirectStiffnessMethod;
using BeamOs.StructuralAnalysis.Domain.OpenSees;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCombinations;
using BeamOs.Tests.Common.SolvedProblems.SAP2000;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace BeamOs.Benchmarks;

[MemoryDiagnoser]
[MediumRunJob]
public class AnalysisBench : IDisposable
{
    private readonly DsmAnalysisModel dsmAnalysisModel;
    private readonly OpenSeesAnalysisModel openSeesAnalysisModel;
    private readonly ISolverFactory solverFactory = new PardisoSolverFactory();
    private readonly LoadCombination loadCombination;

    public AnalysisBench()
    {
        var modelFixture = new TwistyBowlFraming();
        BeamOsModelBuilderDomainMapper mapper = new(modelFixture.Id);
        this.dsmAnalysisModel = mapper.ToDsm(modelFixture, out var model);
        this.loadCombination =
            model.LoadCombinations.FirstOrDefault()
            ?? throw new Exception("Model has no load combinations");
        this.openSeesAnalysisModel = new OpenSeesAnalysisModel(
            model,
            model.Settings.UnitSettings,
            NullLogger.Instance
        );
    }

    [Benchmark]
    public AnalysisResults RunDsm()
    {
        // we need to clone because we are caching some calculated results.
        // those are not copied when we clone the model.
        DsmAnalysisModel analysisModel = this.dsmAnalysisModel.Clone();
        return analysisModel.RunAnalysis(this.solverFactory, this.loadCombination);
    }

    [Benchmark]
    public async Task<AnalysisResults> RunOpenSees()
    {
        return await this.openSeesAnalysisModel.RunAnalysis(this.loadCombination);
    }

    public void Dispose()
    {
        this.openSeesAnalysisModel.Dispose();
    }
}

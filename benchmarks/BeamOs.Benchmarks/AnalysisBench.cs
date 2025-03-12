using BeamOs.StructuralAnalysis.Application.PhysicalModel.Models.Mappers;
using BeamOs.StructuralAnalysis.Domain.DirectStiffnessMethod;
using BeamOs.StructuralAnalysis.Domain.OpenSees;
using BeamOs.Tests.Common.SolvedProblems.SAP2000;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Logging.Abstractions;

namespace BeamOs.Benchmarks;

[MemoryDiagnoser]
[MediumRunJob]
[JsonExporterAttribute.Brief]
public class AnalysisBench : IDisposable
{
    private readonly DsmAnalysisModel dsmAnalysisModel;
    private readonly OpenSeesAnalysisModel openSeesAnalysisModel;

    public AnalysisBench()
    {
        var modelFixture = new TwistyBowlFraming();
        BeamOsModelBuilderDomainMapper mapper = new(modelFixture.Id);
        this.dsmAnalysisModel = mapper.ToDsm(modelFixture, out var model);
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
        return analysisModel.RunAnalysis();
    }

    [Benchmark]
    public async Task<AnalysisResults> RunOpenSees()
    {
        return await this.openSeesAnalysisModel.RunAnalysis();
    }

    public void Dispose()
    {
        this.openSeesAnalysisModel.Dispose();
    }
}

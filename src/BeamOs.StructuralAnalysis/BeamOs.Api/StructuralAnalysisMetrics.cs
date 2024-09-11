using System.Diagnostics.Metrics;

namespace BeamOs.Api;

public class StructuralAnalysisMetrics
{
    public const string MeterName = "BeamOs.Api";
    private const string MeterNamePrefix = "beamos.api.";

    private readonly Meter meter;
    private readonly Dictionary<string, MethodMetrics> methodNameToMetricsDict = [];

    public StructuralAnalysisMetrics(IMeterFactory meterFactory)
    {
        this.meter = meterFactory.Create(MeterName);
    }

    public IDisposable RecordMetrics(string methodName)
    {
        string meterName = GetMeterNameFromMethodName(methodName);
        if (!this.methodNameToMetricsDict.TryGetValue(meterName, out MethodMetrics? metrics))
        {
            metrics = new(
                this.meter.CreateCounter<long>($"{meterName}.count"),
                this.meter.CreateHistogram<double>($"{meterName}.duration")
            );
            this.methodNameToMetricsDict.Add(meterName, metrics);
        }
        return new MethodMetricsRecorder(metrics);
    }

    private static string GetMeterNameFromMethodName(string methodName) =>
        $"{MeterNamePrefix}{methodName}";
}

public sealed record MethodMetrics(Counter<long> RequestCounter, Histogram<double> RequestDuration);

public readonly struct MethodMetricsRecorder(MethodMetrics methodMetrics) : IDisposable
{
    private readonly long requestStartTime = TimeProvider.System.GetTimestamp();

    public void Dispose()
    {
        var elapsed = TimeProvider.System.GetTimestamp() - this.requestStartTime;
        methodMetrics.RequestDuration.Record(elapsed);
        methodMetrics.RequestCounter.Add(1);
    }
}

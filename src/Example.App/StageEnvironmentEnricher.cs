using Serilog.Core;
using Serilog.Events;

namespace Example.App;

public class StageEnvironmentEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var stage = Environment.GetEnvironmentVariable("ASPNETCORE_STAGE");

        if (string.IsNullOrWhiteSpace(stage))
        {
            stage = "dev";
        }

        logEvent.AddPropertyIfAbsent(new LogEventProperty("stage", new ScalarValue(stage)));
    }
}
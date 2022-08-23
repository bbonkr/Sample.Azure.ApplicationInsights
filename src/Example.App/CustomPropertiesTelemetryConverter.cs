using System;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Mvc.Formatters;
using Serilog.Events;
using Serilog.Parsing;
using Serilog.Sinks.ApplicationInsights.TelemetryConverters;

namespace Example.App;

[Obsolete]
public class CustomPropertiesTelemetryConverter : TraceTelemetryConverter // EventTelemetryConverter
{
    public override IEnumerable<ITelemetry> Convert(LogEvent logEvent, IFormatProvider formatProvider)
    {
        //var templateParser = new MessageTemplateParser();
        //var template = templateParser.Parse();

        var customLogEvent = new LogEvent(
            logEvent.Timestamp,
            logEvent.Level,
            logEvent.Exception,
            logEvent.MessageTemplate,
            logEvent.Properties.Select(p => new LogEventProperty(p.Key, p.Value)));

        foreach (var telemetry in base.Convert(customLogEvent, formatProvider))
        {
            yield return telemetry;
        }
    }

    public override ExceptionTelemetry ToExceptionTelemetry(LogEvent logEvent, IFormatProvider formatProvider)
    {
        return base.ToExceptionTelemetry(logEvent, formatProvider);
    }
}

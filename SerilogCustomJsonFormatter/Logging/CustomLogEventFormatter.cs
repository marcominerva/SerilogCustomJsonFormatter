using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SerilogCustomJsonFormatter
{
    public class CustomLogEventFormatter : ITextFormatter
    {
        private static readonly JsonValueFormatter ValueFormatter = new JsonValueFormatter(typeTagName: null);
        private const string COMMA_DELIMITER = ",";

        public static CustomLogEventFormatter Formatter { get; } = new CustomLogEventFormatter();

        public void Format(LogEvent logEvent, TextWriter output)
        {
            if (logEvent == null)
            {
                throw new ArgumentNullException(nameof(logEvent));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            output.Write("{");

            var precedingDelimiter = string.Empty;

            Write(nameof(LogEvent.Timestamp), logEvent.Timestamp.UtcDateTime.ToString("o"));
            Write(nameof(LogEvent.Level), logEvent.Level.ToString());
            Write("Message", logEvent.RenderMessage());

            if (logEvent.Properties.Any())
            {
                output.Write(precedingDelimiter);
                WriteProperties(logEvent.Properties, output);
                precedingDelimiter = COMMA_DELIMITER;
            }

            output.Write("}");

            void Write(string name, string value)
            {
                output.Write(precedingDelimiter);
                precedingDelimiter = COMMA_DELIMITER;
                JsonValueFormatter.WriteQuotedJsonString(name, output);
                output.Write(":");
                JsonValueFormatter.WriteQuotedJsonString(value, output);
            }
        }

        private static void WriteProperties(IReadOnlyDictionary<string, LogEventPropertyValue> properties, TextWriter output)
        {
            output.Write("\"Properties\":{");

            var precedingDelimiter = string.Empty;
            foreach (var property in properties)
            {
                output.Write(precedingDelimiter);
                precedingDelimiter = COMMA_DELIMITER;
                JsonValueFormatter.WriteQuotedJsonString(property.Key, output);
                output.Write(':');
                ValueFormatter.Format(property.Value, output);
            }

            output.Write('}');
        }
    }
}
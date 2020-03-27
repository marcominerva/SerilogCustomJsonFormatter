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

        private const string TIMESTAMP_FIELD = "Timestamp";
        private const string LEVEL_FIELD = "Level";
        private const string MESSAGE_FIELD = "Message";

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

            Write(TIMESTAMP_FIELD, logEvent.Timestamp.ToUniversalTime().DateTime.ToString("o"));
            Write(LEVEL_FIELD, logEvent.Level.ToString());
            Write(MESSAGE_FIELD, logEvent.RenderMessage());

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
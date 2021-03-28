using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HearthStoneWatcher
{
    public class LogParser
    {
        private const string MODE = "mode";
        private const string TIMESTAMP = "timestamp";
        private const string METHOD = "method";
        private const string MESSAGE = "message";

        private static readonly Regex _regex = new Regex(@"^(?<mode>[DWE]) (?<timestamp>[\d:\.]+) (?<method>[a-zA-Z]+\.[a-zA-Z]+\(\))[ :-]+(?<message>.+)$", RegexOptions.Compiled);
        private readonly string[] _seperator;
        private readonly StringSplitOptions _options;
        public LogParser()
        {
            _seperator = new string[] { Environment.NewLine };
            _options = StringSplitOptions.RemoveEmptyEntries;
        }
        public LogLine[] Parse(string chunk)
        {
            var lines = chunk.Split(_seperator, _options);
            var parsedLines = new LogLine[lines.Length];

            for (int i = 0; i < lines.Length; i++)
                parsedLines[i] = ParseLine(lines[i]);

            return parsedLines;
            
        }

        private static LogLine ParseLine(string line)
        {
            var match = _regex.Match(line);
            var mode = '\0';
            var timestamp = DateTime.MinValue;
            var method = string.Empty;
            var message = string.Empty;

            if (match.Groups[MODE].Success)
                mode = match.Groups[MODE].Value[0];
            if (match.Groups[TIMESTAMP].Success && DateTime.TryParse(match.Groups[TIMESTAMP].Value, out DateTime tmp))
                timestamp = tmp;
            if (match.Groups[METHOD].Success)
                method = match.Groups[METHOD].Value;
            if (match.Groups[MESSAGE].Success)
                message = match.Groups[MESSAGE].Value;

            return new LogLine(mode, timestamp, method, message);

        }
    }
}

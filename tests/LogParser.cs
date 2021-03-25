using System;
using System.IO;
using System.Linq;
using Xunit;

namespace HearthStoneWatcher.Tests
{
    public class LogParserTests
    {
        private const string LOG_PATH = "Data/Power.log";

        [Fact]
        public void Parse_AllLinesValid()
        {
            var text = File.ReadAllText(LOG_PATH);

            var parser = new LogParser();
            var parsedLines = parser.Parse(text);

            var failedLines = parsedLines
                .Select((line, index) => new { Line = line, Index = index})
                .Where( x => x.Line.Mode == '\0' || 
                        x.Line.Timestamp == DateTime.MinValue ||
                        x.Line.Method == string.Empty ||
                        x.Line.Message == string.Empty);

            Assert.False(failedLines.Any(), $"There are some lines which aren't parsed properly:\n{string.Join(", ", failedLines.Select(x => x.Index + 1))}");
        }
    }
}

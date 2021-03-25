using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HearthStoneWatcher
{
	public class LogLine
	{
		public LogLine(char mode, DateTime timestamp, string method, string message)
		{
            Mode = mode;
            Timestamp = timestamp;
            Method = method;
            Message = message;
        }

		public char Mode { get; }

		public DateTime Timestamp { get; }
        public string Method { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return $"{Mode} {Timestamp} {Method} - {Message}";
        }
    
	}
}

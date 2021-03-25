using System;

namespace HearthStoneWatcher
{
    public static class Config
    {
        public static string DELIMITER { get; } = Environment.NewLine;
        public static string PATH { get; } = @"C:\Program Files (x86)\Hearthstone\Logs";
        public static string FILE { get; } = "Power.log";
        public static string OPTION_METHOD { get; } = "GameState.DebugPrintOptions()";
    }
}
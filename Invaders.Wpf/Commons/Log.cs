using System;

namespace Invaders.Wpf.Commons
{
    public static class Log
    {
        public static void Info(string message)
        {
            var beforeColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("[Info]>>{0}", message);
            Console.ForegroundColor = beforeColor;
        }

        public static void Debug(string message)
        {
            var beforeColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("[DEBUG]>>{0}", message);
            Console.ForegroundColor = beforeColor;
        }

        public static void Warning(string message)
        {
            var beforeColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[WARNING]>>{0}", message);
            Console.ForegroundColor = beforeColor;
        }

        public static void Strict(string message)
        {
            var foregroundColor = Console.ForegroundColor;
            var backgroundColor = Console.BackgroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.WriteLine("[STRICT]>>{0}", message);
            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;
        }

        public static void Error(string message)
        {
            var foregroundColor = Console.ForegroundColor;
            var backgroundColor = Console.BackgroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.BackgroundColor = ConsoleColor.Red;
            Console.Error.WriteLine("[ERROR]>>{0}", message);
            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;
        }

        public static void Critical(string message)
        {
            var foregroundColor = Console.ForegroundColor;
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine("[CRITICAL]>>{0}", message);
            Console.ForegroundColor = foregroundColor;
        }
    }
}
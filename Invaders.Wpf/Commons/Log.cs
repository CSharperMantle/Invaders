using System;

namespace Invaders.Wpf.Commons
{
    public static class Log
    {
        public static void Info(string message)
        {
            Console.WriteLine("[DEBUG]>>{0}", message);
        }

        public static void Warning(string message)
        {
            Console.WriteLine("[WARNING]>>{0}", message);
        }

        public static void Strict(string message)
        {
            Console.WriteLine("[STRICT]>>{0}", message);
        }

        public static void Error(string message)
        {
            Console.Error.WriteLine("[ERROR]>>{0}", message);
        }

        public static void Critical(string message)
        {
            Console.WriteLine("[CRITICAL]>>{0}", message);
        }
    }
}
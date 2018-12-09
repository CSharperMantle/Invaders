namespace Invaders.Wpf.Commons
{
    public static class Log
    {
        public static void Info(string message)
        {
            System.Diagnostics.Debug.WriteLine("[Info]>>" + message);
        }

        public static void Debug(string message)
        {
            System.Diagnostics.Debug.WriteLine("[DEBUG]>>" + message);
        }

        public static void Warning(string message)
        {
            System.Diagnostics.Debug.WriteLine("[WARNING]>>" + message);
        }

        public static void Strict(string message)
        {
            System.Diagnostics.Debug.WriteLine("[STRICT]>>" + message);
        }

        public static void Error(string message)
        {
            System.Diagnostics.Debug.WriteLine("[ERROR]>>" + message);
        }

        public static void Critical(string message)
        {
            System.Diagnostics.Debug.WriteLine("[CRITICAL]>>" + message);
        }
    }
}
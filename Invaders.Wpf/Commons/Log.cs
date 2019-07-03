using System.Diagnostics;

namespace Invaders.Wpf.Commons
{
    public static class Log
    {
        public static void I(string message)
        {
            Debug.WriteLine($"[INFO]>> {message}");
        }

        public static void D(string message)
        {
            Debug.WriteLine($"[DBG]>> {message}");
        }

        public static void W(string message)
        {
            Debug.WriteLine($"[WARNING]>> {message}");
        }

        public static void E(string message)
        {
            Debug.WriteLine($"[ERROR]>> {message}");
        }

        public static void C(string message)
        {
            Debug.WriteLine($"[CRITICAL]>> {message}");
        }
    }
}
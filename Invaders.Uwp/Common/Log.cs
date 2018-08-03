using System.Diagnostics;

namespace Invaders.Uwp.Commons
{
    public static class Log
    {
        public static void I(string message)
        {
            Debug.WriteLine("[INFO]>{0}", message);
        }

        public static void D(string message)
        {
            Debug.WriteLine("[DEBUG]>{0}", message);
        }

        public static void W(string message)
        {
            Debug.WriteLine("[WARNING]>{0}", message);
        }

        public static void S(string message)
        {
            Debug.WriteLine("[STRICT]>{0}", message);
        }

        public static void E(string message)
        {
            Debug.WriteLine("[ERROR]>{0}", message);
        }

        public static void C(string message)
        {
            Debug.WriteLine("[CRITICAL ERROR]>{0}", message);
        }
    }
}
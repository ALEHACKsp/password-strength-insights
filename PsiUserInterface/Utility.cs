using System;
using System.Globalization;

namespace PsiUserInterface
{
    public class Utility
    {
        public static string GetDate(DateTime timestamp)
        {
            return timestamp.ToString("dd MMMM yyyy", CultureInfo.InvariantCulture);
        }

        public static string GetTime(DateTime timestamp)
        {
            return timestamp.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
        }

        public static string StringOrAlternative(object s, string alternative = "?")
        {
            return (s != null ? s.ToString() : alternative);
        }

        public static string FloatOrAlternative(float s, string alternative = "?")
        {
            return (s != 0.0f ? s.ToString("G6") : alternative);
        }

        public static string DoubleOrAlternative(double d, string alternative = "?")
        {
            return (d != 0.0 ? d.ToString("G6") : alternative);
        }

        public static string ByteStringOrNull(byte[] b, string alternative = "?")
        {
            return (b != null ? BitConverter.ToString(b).Replace("-", "").ToLower() : alternative);
        }
    }
}

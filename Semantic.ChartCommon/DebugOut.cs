using System.Diagnostics;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    public static class DebugOut
    {
        public static DebugMessageTypes AlowedMessageTypes { get; set; }

        static DebugOut()
        {
            DebugOut.AlowedMessageTypes = DebugMessageTypes.None;
        }

        [Conditional("DEBUG")]
        public static void WriteLine(DebugMessageTypes messageType, object value)
        {
            int num = (int)(messageType & DebugOut.AlowedMessageTypes);
        }

        [Conditional("DEBUG")]
        public static void WriteLine(DebugMessageTypes messageType, string message)
        {
            int num = (int)(messageType & DebugOut.AlowedMessageTypes);
        }

        [Conditional("DEBUG")]
        public static void WriteLine(DebugMessageTypes messageType, string format, params object[] args)
        {
            int num = (int)(messageType & DebugOut.AlowedMessageTypes);
        }
    }
}

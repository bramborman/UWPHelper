using System;
using System.Diagnostics;

namespace UWPHelper.Utilities
{
    public static class DebugHelper
    {
        [Conditional("DEBUG")]
        public static void OperationInfo(string objectName, string operationName, bool success)
        {
            Debug.WriteLine($"{objectName} {operationName} {(success ? "succeeded" : "failed")} at {DateTime.Now:HH:mm:ss}");
        }
    }
}

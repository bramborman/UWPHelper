#if DEBUG
using System;
using System.Diagnostics;

namespace UWPHelper.Utilities
{
    public enum Operation
    {
        Saving,
        Loading,
        Syncing
    }
    
    public static class DebugMessages
    {
        public static void OperationInfo(string name, Operation operation, bool success)
        {
            Debug.WriteLine($"{name} {operation.ToString().ToLower()} {(success ? "succeeded" : "failed")} at {DateTime.Now.Hour:D2}:{DateTime.Now.Minute:D2}:{DateTime.Now.Second:D2}");
        }
    }
}
#endif
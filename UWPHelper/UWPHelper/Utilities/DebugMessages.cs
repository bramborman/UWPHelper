using System;

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
        public static string OperationInfo(string name, Operation operation, bool success)
        {
            return $"{name} {operation.ToString().ToLower()} {(success ? "succeeded" : "failed")} at {DateTime.Now.Hour:D2}:{DateTime.Now.Minute:D2}:{DateTime.Now.Second:D2}";
        }
    }
}
using System;
using System.Diagnostics;

namespace UWPHelper.Utilities
{
    public static class DebugHelper
    {
        [Conditional("DEBUG")]
        internal static void OperationInfo(string objectName, string operationName, bool success)
        {
            Debug.WriteLine(GetOperationInfoString(objectName, operationName, success));
        }

        public static string GetOperationInfoString(string objectName, string operationName, bool success)
        {
            return GetTimedMessageString($"{objectName} {operationName} {(success ? "succeeded" : "failed")}");
        }

        public static string GetTimedMessageString(string message)
        {
            return GetTimedMessageString(message, "HH:mm:ss");
        }

        public static string GetTimedMessageString(string message, string dateTimeFormatting)
        {
            ExceptionHelper.ValidateNotNullOrWhiteSpace(dateTimeFormatting, nameof(dateTimeFormatting));
            return DateTime.Now.ToString(dateTimeFormatting) + " - " + message;
        }
    }
}

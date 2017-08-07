using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;

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
            ExceptionHelper.ValidateStringNotNullOrWhiteSpace(dateTimeFormatting, nameof(dateTimeFormatting));
            return DateTime.Now.ToString(dateTimeFormatting) + " - " + message;
        }
        
        public static void RegisterDebugPropertyChangedEventHandler(INotifyPropertyChanged obj)
        {
            obj.PropertyChanged += (sender, e) =>
            {
                Debug.WriteLine($"{sender.GetType().Name}: {e.PropertyName} changed to {sender.GetTypeInfo().GetRuntimeProperty(e.PropertyName).GetValue(sender)}.");
            };
        }
    }
}

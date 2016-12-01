using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace UWPHelper.Utilities
{
    public static class KeyboardHelper
    {
        public static bool IsDown(VirtualKey key)
        {
            return IsInState(key, CoreVirtualKeyStates.Down);
        }

        public static bool IsInState(VirtualKey key, CoreVirtualKeyStates state)
        {
            return Window.Current.CoreWindow.GetKeyState(key).HasFlag(state);
        }
    }
}

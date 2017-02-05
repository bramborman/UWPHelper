using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace UWPHelper.Utilities
{
    public static class KeyboardHelper
    {
        public static event TypedEventHandler<CoreWindow, KeyEventArgs> CoreKeyDown
        {
            add { Window.Current.CoreWindow.KeyDown += value; }
            remove { Window.Current.CoreWindow.KeyDown -= value; }
        }
        public static event TypedEventHandler<CoreWindow, KeyEventArgs> CoreKeyUp
        {
            add { Window.Current.CoreWindow.KeyUp += value; }
            remove { Window.Current.CoreWindow.KeyUp -= value; }
        }

        public static bool IsKeyDown(VirtualKey key)
        {
            return IsKeyInState(key, CoreVirtualKeyStates.Down);
        }

        public static bool IsKeyInState(VirtualKey key, CoreVirtualKeyStates state)
        {
            return Window.Current.CoreWindow.GetKeyState(key).HasFlag(state);
        }
    }
}

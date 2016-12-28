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

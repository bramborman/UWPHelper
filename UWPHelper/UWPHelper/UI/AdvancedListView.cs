using UWPHelper.Utilities;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace UWPHelper.UI
{
    public class AdvancedListView : ListView
    {
        public static readonly DependencyProperty IsContextMenuEnabledProperty = DependencyProperty.Register(nameof(IsContextMenuEnabled), typeof(bool), typeof(AdvancedListView), new PropertyMetadata(false, IsContextMenuEnabledPropertyChanged));
        
        public bool IsContextMenuEnabled
        {
            get { return (bool)GetValue(IsContextMenuEnabledProperty); }
            set { SetValue(IsContextMenuEnabledProperty, value); }
        }

        public object FocusedItem { get; private set; }

        private void OpenContextMenu(object sender, RightTappedRoutedEventArgs e)
        {
            if (e.OriginalSource != this)
            {
                UIElement tappedItem = (UIElement)e.OriginalSource;
                OpenContextMenu(tappedItem, e.GetPosition(tappedItem));
            }
        }

        private void OpenContextMenu(UIElement targetElement, Point point)
        {
            ((MenuFlyout)FlyoutBase.GetAttachedFlyout(this)).ShowAt(targetElement, point);
        }

        private void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            if (args.Handled || AdvancedContentDialog.IsOpened)
            {
                return;
            }

            if (args.VirtualKey == VirtualKey.Application)
            {
                if (FocusManager.GetFocusedElement() is ListViewItem focusedElement)
                {
                    args.Handled = true;
                    FocusedItem = focusedElement.Content;
                    OpenContextMenu(focusedElement, new Point(focusedElement.ActualWidth / 2, focusedElement.ActualHeight / 2));
                }
            }
        }

        private static void IsContextMenuEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AdvancedListView advancedListView = (AdvancedListView)d;

            if ((bool)e.NewValue)
            {
                advancedListView.RightTapped += advancedListView.OpenContextMenu;
                KeyboardHelper.CoreKeyDown   += advancedListView.CoreWindow_KeyDown;
            }
            else
            {
                advancedListView.RightTapped -= advancedListView.OpenContextMenu;
                KeyboardHelper.CoreKeyDown   -= advancedListView.CoreWindow_KeyDown;
            }
        }
    }
}

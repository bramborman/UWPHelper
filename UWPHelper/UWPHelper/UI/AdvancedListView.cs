using Windows.Foundation;
using Windows.System;
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

        private void OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Handled || AdvancedContentDialog.IsOpened)
            {
                return;
            }

            if (e.Key == VirtualKey.Application)
            {
                if (FocusManager.GetFocusedElement() is ListViewItem focusedElement)
                {
                    e.Handled = true;
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
                advancedListView.KeyDown     += advancedListView.OnKeyDown;
            }
            else
            {
                advancedListView.RightTapped -= advancedListView.OpenContextMenu;
                advancedListView.KeyDown     -= advancedListView.OnKeyDown;
            }
        }
    }
}

using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Navigation;

namespace UWPHelper.UI
{
    [ContentProperty(Name = nameof(PrimaryItems))]
    public sealed partial class HamburgerMenu : UserControl
    {
        internal const double DEFAULT_ICON_WIDTH = 48.0;

        public static readonly DependencyProperty IsBackButtonEnabledProperty   = DependencyProperty.Register(nameof(IsBackButtonEnabled), typeof(bool), typeof(HamburgerMenu), new PropertyMetadata(false, IsBackButtonEnabledPropertyChanged));
        public static readonly DependencyProperty DisplayModeProperty           = DependencyProperty.Register(nameof(DisplayMode), typeof(SplitViewDisplayMode), typeof(HamburgerMenu), new PropertyMetadata(SplitViewDisplayMode.Overlay, DisplayModePropertyChanged));
        public static readonly DependencyProperty CompactPaneLengthProperty     = DependencyProperty.Register(nameof(CompactPaneLength), typeof(double), typeof(HamburgerMenu), new PropertyMetadata(DEFAULT_ICON_WIDTH));
        public static readonly DependencyProperty OpenPaneWidthProperty         = DependencyProperty.Register(nameof(OpenPaneLength), typeof(double), typeof(HamburgerMenu), new PropertyMetadata(320.0));
        public static readonly DependencyProperty InitialPageTypeProperty       = DependencyProperty.Register(nameof(InitialPageType), typeof(Type), typeof(HamburgerMenu), null);
        public static readonly DependencyProperty HeaderVisibilityProperty      = DependencyProperty.Register(nameof(HeaderVisibility), typeof(Visibility), typeof(HamburgerMenu), null);
        
        private bool navigationLocked;
        private HamburgerMenuItem selectedHamburgerMenuItem;
        private SystemNavigationManager _systemNavigationManager;

        private SystemNavigationManager SystemNavigationManager
        {
            get
            {
                if (_systemNavigationManager == null)
                {
                    _systemNavigationManager = SystemNavigationManager.GetForCurrentView();
                }

                return _systemNavigationManager;
            }
        }
        
        public bool IsBackButtonEnabled
        {
            get { return (bool)GetValue(IsBackButtonEnabledProperty); }
            set { SetValue(IsBackButtonEnabledProperty, value); }
        }
        public SplitViewDisplayMode DisplayMode
        {
            get { return (SplitViewDisplayMode)GetValue(DisplayModeProperty); }
            set { SetValue(DisplayModeProperty, value); }
        }
        public double CompactPaneLength
        {
            get { return (double)GetValue(CompactPaneLengthProperty); }
            set { SetValue(CompactPaneLengthProperty, value); }
        }
        public double OpenPaneLength
        {
            get { return (double)GetValue(OpenPaneWidthProperty); }
            set { SetValue(OpenPaneWidthProperty, value); }
        }
        public Type InitialPageType
        {
            get { return (Type)GetValue(InitialPageTypeProperty); }
            set { SetValue(InitialPageTypeProperty, value); }
        }
        public Visibility HeaderVisibility
        {
            get { return (Visibility)GetValue(HeaderVisibilityProperty); }
            set { SetValue(HeaderVisibilityProperty, value); }
        }

        public Frame ContentFrame
        {
            get { return Fr_Content; }
        }
        public ObservableCollection<HamburgerMenuItem> PrimaryItems { get; }
        public ObservableCollection<HamburgerMenuItem> SecondaryItems { get; }

        public event RoutedEventHandler Navigated;
        public event RoutedEventHandler Navigating;    
            
        public HamburgerMenu()
        {
            PrimaryItems    = new ObservableCollection<HamburgerMenuItem>();
            SecondaryItems  = new ObservableCollection<HamburgerMenuItem>();

            InitializeComponent();
        }

        private void ToggleHamburgerMenu(object sender, RoutedEventArgs e)
        {
            SV_HamburgerMenu.IsPaneOpen = !SV_HamburgerMenu.IsPaneOpen;
        }

        public void Navigate(Type pageType)
        {
            Fr_Content.Navigate(pageType);
        }
        
        private void Navigate(object sender, SelectionChangedEventArgs e)
        {
            if (!navigationLocked)
            {
                HamburgerMenuItem selectedHamburgerMenuItem = (HamburgerMenuItem)((ListView)sender).SelectedItem;
                Fr_Content.Navigate(selectedHamburgerMenuItem.PageType);
            }
        }

        private void Fr_Content_Loaded(object sender, RoutedEventArgs e)
        {
            if (Fr_Content.Content == null)
            {
                Fr_Content.Navigate(InitialPageType);
            }
        }

        private void Fr_Content_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            Navigating?.Invoke(this, new RoutedEventArgs());

            if (Fr_Content.Content != null)
            {
                // Remove highlight from previously selected item in hamburger menu
                navigationLocked = true;
                selectedHamburgerMenuItem.IsSelected = false;
                navigationLocked = false;
            }
        }

        private void Fr_Content_Navigated(object sender, NavigationEventArgs e)
        {
            SetBackButtonVisibility();
            navigationLocked = true;

            // Highlight the right item in hamburger menu using SelectedIndex (using SelectedItem was buggy)
            Type pageType = Fr_Content.Content.GetType();
            selectedHamburgerMenuItem = PrimaryItems.FirstOrDefault(i => i.PageType == pageType) ?? SecondaryItems.First(i => i.PageType == pageType);

            ListView activeHamburgerMenu      = PrimaryItems.Contains(selectedHamburgerMenuItem) ? LV_PrimaryItems : LV_SecondaryItems;
            activeHamburgerMenu.SelectedIndex = activeHamburgerMenu.Items.IndexOf(selectedHamburgerMenuItem);

            navigationLocked = false;
            SV_HamburgerMenu.IsPaneOpen = false;
            
            Navigated?.Invoke(this, new RoutedEventArgs());
        }

        private void SetBackButtonVisibility()
        {
            SystemNavigationManager.AppViewBackButtonVisibility = Fr_Content.CanGoBack && IsBackButtonEnabled ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            if (Fr_Content.CanGoBack && IsBackButtonEnabled)
            {
                e.Handled = true;
                Fr_Content.GoBack();
            }
        }

        private static void IsBackButtonEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HamburgerMenu hamburgerMenu = (HamburgerMenu)d;
            ((HamburgerMenu)d).SetBackButtonVisibility();

            if ((bool)e.NewValue)
            {
                hamburgerMenu.SystemNavigationManager.BackRequested += hamburgerMenu.OnBackRequested;
            }
            else
            {
                hamburgerMenu.SystemNavigationManager.BackRequested -= hamburgerMenu.OnBackRequested;
            }
        }

        private static void DisplayModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SplitViewDisplayMode displayMode = (SplitViewDisplayMode)e.NewValue;
            HamburgerMenu hamburgerMenu = (HamburgerMenu)d;

            if (displayMode == SplitViewDisplayMode.CompactInline || displayMode == SplitViewDisplayMode.CompactOverlay)
            {
                hamburgerMenu.CP_PageTitle.Padding = (Thickness)hamburgerMenu.Resources["HamburgerMenuCompactDisplayModePageTitlePadding"];
            }
            else
            {
                hamburgerMenu.CP_PageTitle.Padding = (Thickness)hamburgerMenu.Resources["HamburgerMenuPageTitlePadding"];
            }
        }
    }
}

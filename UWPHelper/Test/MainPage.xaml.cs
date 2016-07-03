using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Test
{
    public enum TestEnum
    {
        _0,
        _1,
        _2,
        _3
    }

    public sealed partial class MainPage : Page
    {
        public TestEnum TestEnum { get; set; }

        public MainPage()
        {
            InitializeComponent();
        }
    }
}

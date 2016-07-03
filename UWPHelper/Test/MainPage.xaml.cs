using Windows.UI.Xaml.Controls;

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

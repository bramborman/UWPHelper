namespace UWPHelper.Utilities
{
    public class HandledEventArgs
    {
        public bool Handled { get; set; }

        public HandledEventArgs()
        {
            Handled = false;
        }
    }
}
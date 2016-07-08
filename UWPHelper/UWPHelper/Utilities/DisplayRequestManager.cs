using Windows.System.Display;

namespace UWPHelper.Utilities
{
    public static class DisplayRequestManager
    {
        static DisplayRequest displayRequest;

        static bool _isActive;

        public static bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;

                    if (_isActive)
                    {
                        if (displayRequest == null)
                        {
                            displayRequest = new DisplayRequest();
                        }

                        displayRequest.RequestActive();
                    }
                    else
                    {
                        displayRequest.RequestRelease();
                    }
                }
            }
        }
    }
}
using Windows.System.Display;

namespace UWPHelper
{
    public class DisplayRequestManager
    {
        DisplayRequest displayRequest;

        bool _isActive;

        public bool IsActive
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
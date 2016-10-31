using System;
using System.Threading;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace UWPHelper.Utilities
{
    public sealed class ThreadPoolTimer
    {
        bool invokeTickOnDispatcher;
        Timer timer;
        TimeSpan _interval;

        public bool IsEnabled { get; private set; }
        public TimeSpan Interval
        {
            get { return _interval; }
            set
            {
                if (_interval != value)
                {
                    _interval = value;

                    if (IsEnabled)
                    {
                        SetTimer();
                    }
                }
            }
        }

        public event EventHandler Tick;

        public ThreadPoolTimer(bool invokeTickOnDispatcher)
        {
            this.invokeTickOnDispatcher = invokeTickOnDispatcher;
        }

        private void SetTimer()
        {
            timer.Change(new TimeSpan(0), Interval);
        }

        public void Start()
        {
            if (IsEnabled)
            {
                SetTimer();
            }
            else
            {
                timer = new Timer(
                    async state =>
                    {
                        if (invokeTickOnDispatcher)
                        {
                            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Tick?.Invoke(this, new EventArgs()));
                        }
                        else
                        {
                            Tick?.Invoke(this, new EventArgs());
                        }
                    }, null, new TimeSpan(0), Interval);
                IsEnabled = true;
            }
        }

        public void Stop()
        {
            IsEnabled = false;
            timer.Dispose();
        }
    }
}

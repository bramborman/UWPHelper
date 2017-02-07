using System;
using System.Threading;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace UWPHelper.Utilities
{
    public sealed class ThreadPoolTimer
    {
        private Timer timer;
        private TimeSpan _interval;

        private bool InvokeOnDispatcher { get; }
        public bool IsEnabled { get; private set; }
        public TimeSpan Interval
        {
            get { return _interval; }
            set
            {
                if (value < TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

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

        public ThreadPoolTimer(bool invokeOnDispatcher)
        {
            InvokeOnDispatcher = invokeOnDispatcher;
        }

        private void SetTimer()
        {
            lock (timer)
            {
                timer.Change(new TimeSpan(0), Interval);
            }
        }

        public void Start()
        {
            if (IsEnabled)
            {
                SetTimer();
            }
            else
            {
                TimerCallback timerCallback;

                if (!InvokeOnDispatcher)
                {
                    timerCallback = state =>
                    {
                        Tick?.Invoke(this, new EventArgs());
                    };
                }
                else
                {
                    timerCallback = async state =>
                    {
                        await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Tick?.Invoke(this, new EventArgs()));
                    };
                }

                timer = new Timer(timerCallback, null, new TimeSpan(0), Interval);
                IsEnabled = true;
            }
        }

        public void Stop()
        {
            IsEnabled = false;

            lock (timer)
            {
                timer.Dispose();
                timer = null;
            }
        }
    }
}

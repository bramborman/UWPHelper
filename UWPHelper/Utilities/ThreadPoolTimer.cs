using System;
using System.Diagnostics;
using System.Threading;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace UWPHelper.Utilities
{
    [DebuggerDisplay("IsEnabled = {IsEnabled}")]
    public sealed class ThreadPoolTimer : IDisposable
    {
        private readonly TimerCallback timerCallback;

        private Timer timer;
        private TimeSpan _interval;

        public bool IsTickInvokedOnMainViewDispatcher { get; }
        public bool IsDisposedOnStop { get; }
        public bool IsEnabled { get; private set; }
        public CoreDispatcher Dispatcher { get; }
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
                        SetTimerInterval();
                    }
                }
            }
        }

        public event EventHandler Tick;

        public ThreadPoolTimer() : this(TimeSpan.Zero)
        {

        }
        
        public ThreadPoolTimer(TimeSpan interval) : this(interval, true, true)
        {
            
        }

        public ThreadPoolTimer(TimeSpan interval, bool invokeTickOnMainViewDispatcher, bool disposeOnStop) : this(interval, CoreApplication.MainView.CoreWindow.Dispatcher, invokeTickOnMainViewDispatcher, disposeOnStop)
        {

        }

        public ThreadPoolTimer(TimeSpan interval, CoreDispatcher dispatcher) : this(interval, dispatcher, true, true)
        {

        }
        
        public ThreadPoolTimer(TimeSpan interval, CoreDispatcher dispatcher, bool invokeTickOnMainViewDispatcher, bool disposeOnStop)
        {
            ExceptionHelper.ValidateObjectNotNull(dispatcher, nameof(dispatcher));

            Interval                            = interval;
            Dispatcher                          = dispatcher;
            IsTickInvokedOnMainViewDispatcher   = invokeTickOnMainViewDispatcher;
            IsDisposedOnStop                    = disposeOnStop;

            if (invokeTickOnMainViewDispatcher)
            {
                timerCallback = async state =>
                {
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        Tick?.Invoke(this, new EventArgs());
                    });
                };
            }
            else
            {
                timerCallback = state =>
                {
                    Tick?.Invoke(this, new EventArgs());
                };
            }
        }

        private void SetTimerInterval()
        {
            timer.Change(TimeSpan.Zero, Interval);
        }

        public void Start()
        {
            if (IsEnabled)
            {
                return;
            }

            if (timer == null)
            {
                timer = new Timer(timerCallback, null, TimeSpan.Zero, Interval);
            }
            else
            {
                SetTimerInterval();
            }

            IsEnabled = true;
        }

        public void Stop()
        {
            Stop(false);
        }

        public void Dispose()
        {
            Stop(true);
        }

        private void Stop(bool forceDispose)
        {
            IsEnabled = false;

            if (IsDisposedOnStop || forceDispose)
            {
                timer?.Dispose();
                timer = null;
            }
            else
            {
                timer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }
    }
}

using System;
#if DEBUG
using System.Diagnostics;
#endif

namespace UWPHelper.Utilities
{
    public sealed class OneEventHandlerHelper
    {
#if DEBUG
        internal string Name { get; }
#endif

        public int HandlersCount { get; private set; }

        public OneEventHandlerHelper()
        {

        }

#if DEBUG
        internal OneEventHandlerHelper(string name)
        {
            Name = name;
        }
#endif

        public bool AddHandler(Action attachHandlerToEvent)
        {
            ExceptionHelper.ValidateNotNull(attachHandlerToEvent, nameof(attachHandlerToEvent));
#if DEBUG
            Debug.WriteLine($"{Name}: {nameof(AddHandler)} called");
#endif

            if (HandlersCount++ == 0)
            {
                attachHandlerToEvent();
#if DEBUG
                Debug.WriteLine($"{Name}: event attached");
#endif

                return true;
            }

            return false;
        }

        public bool RemoveHandler(Action unattachHandlerFromEvent)
        {
            ExceptionHelper.ValidateNotNull(unattachHandlerFromEvent, nameof(unattachHandlerFromEvent));
#if DEBUG
            Debug.WriteLine($"{Name}: {nameof(RemoveHandler)} called");
#endif

            if (--HandlersCount == 0)
            {
                unattachHandlerFromEvent();
#if DEBUG
                Debug.WriteLine($"{Name}: event unattached");
#endif

                return true;
            }

            return false;
        }
    }
}

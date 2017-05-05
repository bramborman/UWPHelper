using System;
using System.Diagnostics;

namespace UWPHelper.Utilities
{
    public sealed class OneEventHandlerHelper
    {
        internal string Name { get; }

        public int HandlersCount { get; private set; }

        public OneEventHandlerHelper()
        {

        }
        
        internal OneEventHandlerHelper(string name)
        {
            Name = name;
        }

        public bool AddHandler(Action attachHandlerToEvent)
        {
            ExceptionHelper.ValidateObjectNotNull(attachHandlerToEvent, nameof(attachHandlerToEvent));
            Debug.WriteLine($"{Name} - {nameof(AddHandler)} called");

            if (HandlersCount++ == 0)
            {
                attachHandlerToEvent();
                Debug.WriteLine(Name + " - event attached");

                return true;
            }

            return false;
        }

        public bool RemoveHandler(Action unattachHandlerFromEvent)
        {
            ExceptionHelper.ValidateObjectNotNull(unattachHandlerFromEvent, nameof(unattachHandlerFromEvent));
            Debug.WriteLine($"{Name} - {nameof(RemoveHandler)} called");

            if (--HandlersCount == 0)
            {
                unattachHandlerFromEvent();
                Debug.WriteLine(Name + " - event unattached");

                return true;
            }

            return false;
        }
    }
}

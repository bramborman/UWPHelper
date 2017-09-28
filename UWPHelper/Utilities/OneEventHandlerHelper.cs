using System;

namespace UWPHelper.Utilities
{
    public sealed class OneEventHandlerHelper
    {
        public int HandlersCount { get; private set; }

        public bool AddHandler(Action attachHandlerToEvent)
        {
            ExceptionHelper.ValidateObjectNotNull(attachHandlerToEvent, nameof(attachHandlerToEvent));

            if (HandlersCount++ == 0)
            {
                attachHandlerToEvent();
                return true;
            }

            return false;
        }

        public bool RemoveHandler(Action unattachHandlerFromEvent)
        {
            ExceptionHelper.ValidateObjectNotNull(unattachHandlerFromEvent, nameof(unattachHandlerFromEvent));

            if (--HandlersCount == 0)
            {
                unattachHandlerFromEvent();
                return true;
            }

            return false;
        }
    }
}

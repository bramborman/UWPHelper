using System;

namespace UWPHelper.Utilities
{
    public sealed class OneEventHandlerHelper
    {
        public int HandlersCount { get; private set; }

        public bool AddHandler(Action attachHandlerToEvent)
        {
            ValidateAcion(attachHandlerToEvent, nameof(attachHandlerToEvent));

            if (HandlersCount++ == 0)
            {
                attachHandlerToEvent();
                return true;
            }

            return false;
        }

        public bool RemoveHandler(Action unattachHandlerFromEvent)
        {
            ValidateAcion(unattachHandlerFromEvent, nameof(unattachHandlerFromEvent));

            if (--HandlersCount == 0)
            {
                unattachHandlerFromEvent();
                return true;
            }

            return false;
        }

        private void ValidateAcion(Action action, string parameterName)
        {
            if (action == null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }
    }
}

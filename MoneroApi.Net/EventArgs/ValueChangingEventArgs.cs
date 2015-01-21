using System;

namespace Jojatekok.MoneroAPI
{
    public class ValueChangingEventArgs<T> : EventArgs
    {
        public T NewValue { get; private set; }
        public T OldValue { get; private set; }

        internal ValueChangingEventArgs(T newValue, T oldValue)
        {
            NewValue = newValue;
            OldValue = oldValue;
        }
    }
}

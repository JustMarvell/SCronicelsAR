using System;
using System.Collections.Generic;

namespace Scar.Core
{
    public static class EventBus
    {
        static readonly Dictionary<Type, Delegate> s_Events = new();

        public static void Subscribe<T>(Action<T> handler)
        {
            var t = typeof(T);
            s_Events[t] = s_Events.TryGetValue(t, out var e) ? Delegate.Combine(e, handler) : handler;
        }

        public static void Unsubscribe<T>(Action<T> handler)
        {
            var t = typeof(T);
            if (s_Events.TryGetValue(t, out var e))
                s_Events[t] = Delegate.Remove(e, handler);
        }

        public static void Publish<T>(T evt)
        {
            if (s_Events.TryGetValue(typeof(T), out var h))
                ((Action<T>)h)?.Invoke(evt);
        }
    }
}
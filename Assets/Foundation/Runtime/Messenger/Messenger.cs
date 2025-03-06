using System;
using System.Collections.Generic;

namespace DarkNaku.Messenger
{
    public static class Messenger
    {
        public static void AddListener<T>(Action<T> listener) where T : struct
        {
            MessengerInternal<T>.AddListener(listener);
        }
        
        public static void RemoveListener<T>(Action<T> listener) where T : struct
        {
            MessengerInternal<T>.RemoveListener(listener);
        }
        
        public static void Clear<T>() where T : struct
        {
            MessengerInternal<T>.Clear();
        }
        
        public static void Broadcast<T>(T @event) where T : struct
        {
            MessengerInternal<T>.Broadcast(@event);
        }
        
        internal static class MessengerInternal<T> where T : struct
        {
            private static Action<T> _listeners = _ => { };
        
            public static void AddListener(Action<T> listener)
            {
                _listeners += listener;
            }
        
            public static void RemoveListener(Action<T> listener)
            {
                _listeners -= listener;
            }

            public static void Clear()
            {
                _listeners = _ => { };
            }
        
            public static void Broadcast(T @event)
            {
                _listeners.Invoke(@event);
            }
        }
    }
}
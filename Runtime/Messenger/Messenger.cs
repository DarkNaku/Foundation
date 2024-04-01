using System;
using System.Collections.Generic;

namespace DarkNaku.Messenger
{
    public static class Messenger<T>
    {
        private static readonly Dictionary<T, Delegate> _eventTable = MessengerInternal<T>.EventTable;

        public static void AddListener(T eventType, OnBroadcast handler)
        {
            MessengerInternal<T>.AddListener(eventType, handler);
            _eventTable[eventType] = (OnBroadcast)_eventTable[eventType] + handler;
        }

        public static void RemoveListener(T eventType, OnBroadcast handler)
        {
            MessengerInternal<T>.RemoveListener(eventType, handler);
            _eventTable[eventType] = (OnBroadcast)_eventTable[eventType] - handler;
            MessengerInternal<T>.OnRemoveListener(eventType);
        }

        public static void Broadcast(T eventType)
        {
            Broadcast(eventType, MessengerInternal<T>.DEFAULT_MODE);
        }

        private static void Broadcast(T eventType, MessengerMode mode)
        {
            MessengerInternal<T>.OnBroadcasting(eventType, mode);

            if (_eventTable.TryGetValue(eventType, out var d))
            {
                if (d is OnBroadcast callback)
                {
                    callback();
                }
                else
                {
                    throw MessengerInternal<T>.CreateBroadcastSignatureException(eventType);
                }
            }
        }
    }

    public static class Messenger<T, U0>
    {
        private static readonly Dictionary<T, Delegate> _eventTable = MessengerInternal<T>.EventTable;

        public static void AddListener(T eventType, OnBroadcast<U0> handler)
        {
            MessengerInternal<T>.AddListener(eventType, handler);
            _eventTable[eventType] = (OnBroadcast<U0>)_eventTable[eventType] + handler;
        }

        public static void RemoveListener(T eventType, OnBroadcast<U0> handler)
        {
            MessengerInternal<T>.RemoveListener(eventType, handler);
            _eventTable[eventType] = (OnBroadcast<U0>)_eventTable[eventType] - handler;
            MessengerInternal<T>.OnRemoveListener(eventType);
        }

        public static void Broadcast(T eventType, U0 param0)
        {
            Broadcast(eventType, param0, MessengerInternal<T>.DEFAULT_MODE);
        }

        private static void Broadcast(T eventType, U0 param0, MessengerMode mode)
        {
            MessengerInternal<T>.OnBroadcasting(eventType, mode);

            if (!_eventTable.TryGetValue(eventType, out var d)) return;

            if (d is OnBroadcast<U0> callback)
            {
                callback(param0);
            }
            else
            {
                throw MessengerInternal<T>.CreateBroadcastSignatureException(eventType);
            }
        }
    }

    public static class Messenger<T, U0, U1>
    {
        private static readonly Dictionary<T, Delegate> _eventTable = MessengerInternal<T>.EventTable;

        public static void AddListener(T eventType, OnBroadcast<U0, U1> handler)
        {
            MessengerInternal<T>.AddListener(eventType, handler);
            _eventTable[eventType] = (OnBroadcast<U0, U1>)_eventTable[eventType] + handler;
        }

        public static void RemoveListener(T eventType, OnBroadcast<U0, U1> handler)
        {
            MessengerInternal<T>.RemoveListener(eventType, handler);
            _eventTable[eventType] = (OnBroadcast<U0, U1>)_eventTable[eventType] - handler;
            MessengerInternal<T>.OnRemoveListener(eventType);
        }

        public static void Broadcast(T eventType, U0 param0, U1 param1)
        {
            Broadcast(eventType, param0, param1, MessengerInternal<T>.DEFAULT_MODE);
        }

        private static void Broadcast(T eventType, U0 param0, U1 param1, MessengerMode mode)
        {
            MessengerInternal<T>.OnBroadcasting(eventType, mode);

            if (!_eventTable.TryGetValue(eventType, out var d)) return;

            if (d is OnBroadcast<U0, U1> callback)
            {
                callback(param0, param1);
            }
            else
            {
                throw MessengerInternal<T>.CreateBroadcastSignatureException(eventType);
            }
        }
    }

    public static class Messenger<T, U0, U1, U2>
    {
        private static Dictionary<T, Delegate> eventTable = MessengerInternal<T>.EventTable;

        public static void AddListener(T eventType, OnBroadcast<U0, U1, U2> handler)
        {
            MessengerInternal<T>.AddListener(eventType, handler);
            eventTable[eventType] = (OnBroadcast<U0, U1, U2>)eventTable[eventType] + handler;
        }

        public static void RemoveListener(T eventType, OnBroadcast<U0, U1, U2> handler)
        {
            MessengerInternal<T>.RemoveListener(eventType, handler);
            eventTable[eventType] = (OnBroadcast<U0, U1, U2>)eventTable[eventType] - handler;
            MessengerInternal<T>.OnRemoveListener(eventType);
        }

        public static void Broadcast(T eventType, U0 param0, U1 param1, U2 param2)
        {
            Broadcast(eventType, param0, param1, param2, MessengerInternal<T>.DEFAULT_MODE);
        }

        private static void Broadcast(T eventType, U0 param0, U1 param1, U2 param2, MessengerMode mode)
        {
            MessengerInternal<T>.OnBroadcasting(eventType, mode);

            if (!eventTable.TryGetValue(eventType, out var d)) return;

            if (d is OnBroadcast<U0, U1, U2> callback)
            {
                callback(param0, param1, param2);
            }
            else
            {
                throw MessengerInternal<T>.CreateBroadcastSignatureException(eventType);
            }
        }
    }
}
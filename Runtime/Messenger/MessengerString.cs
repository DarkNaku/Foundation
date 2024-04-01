using System;
using System.Collections.Generic;

namespace DarkNaku.Messenger
{
    public static class MessengerString
    {
        private static readonly Dictionary<string, Delegate> _eventTable = MessengerInternal<string>.EventTable;

        public static void AddListener(string eventType, OnBroadcast handler)
        {
            MessengerInternal<string>.AddListener(eventType, handler);
            _eventTable[eventType] = (OnBroadcast)_eventTable[eventType] + handler;
        }

        public static void RemoveListener(string eventType, OnBroadcast handler)
        {
            MessengerInternal<string>.RemoveListener(eventType, handler);
            _eventTable[eventType] = (OnBroadcast)_eventTable[eventType] - handler;
            MessengerInternal<string>.OnRemoveListener(eventType);
        }

        public static void Broadcast(string eventType)
        {
            Broadcast(eventType, MessengerInternal<string>.DEFAULT_MODE);
        }

        private static void Broadcast(string eventType, MessengerMode mode)
        {
            MessengerInternal<string>.OnBroadcasting(eventType, mode);

            if (_eventTable.TryGetValue(eventType, out var d))
            {
                if (d is OnBroadcast callback)
                {
                    callback();
                }
                else
                {
                    throw MessengerInternal<string>.CreateBroadcastSignatureException(eventType);
                }
            }
        }
    }

    public static class MessengerString<U0>
    {
        private static readonly Dictionary<string, Delegate> _eventTable = MessengerInternal<string>.EventTable;

        public static void AddListener(string eventType, OnBroadcast<U0> handler)
        {
            MessengerInternal<string>.AddListener(eventType, handler);
            _eventTable[eventType] = (OnBroadcast<U0>)_eventTable[eventType] + handler;
        }

        public static void RemoveListener(string eventType, OnBroadcast<U0> handler)
        {
            MessengerInternal<string>.RemoveListener(eventType, handler);
            _eventTable[eventType] = (OnBroadcast<U0>)_eventTable[eventType] - handler;
            MessengerInternal<string>.OnRemoveListener(eventType);
        }

        public static void Broadcast(string eventType, U0 param0)
        {
            Broadcast(eventType, param0, MessengerInternal<string>.DEFAULT_MODE);
        }

        private static void Broadcast(string eventType, U0 param0, MessengerMode mode)
        {
            MessengerInternal<string>.OnBroadcasting(eventType, mode);

            if (!_eventTable.TryGetValue(eventType, out var d)) return;

            if (d is OnBroadcast<U0> callback)
            {
                callback(param0);
            }
            else
            {
                throw MessengerInternal<string>.CreateBroadcastSignatureException(eventType);
            }
        }
    }

    public static class MessengerString<U0, U1>
    {
        private static readonly Dictionary<string, Delegate> _eventTable = MessengerInternal<string>.EventTable;

        public static void AddListener(string eventType, OnBroadcast<U0, U1> handler)
        {
            MessengerInternal<string>.AddListener(eventType, handler);
            _eventTable[eventType] = (OnBroadcast<U0, U1>)_eventTable[eventType] + handler;
        }

        public static void RemoveListener(string eventType, OnBroadcast<U0, U1> handler)
        {
            MessengerInternal<string>.RemoveListener(eventType, handler);
            _eventTable[eventType] = (OnBroadcast<U0, U1>)_eventTable[eventType] - handler;
            MessengerInternal<string>.OnRemoveListener(eventType);
        }

        public static void Broadcast(string eventType, U0 param0, U1 param1)
        {
            Broadcast(eventType, param0, param1, MessengerInternal<string>.DEFAULT_MODE);
        }

        private static void Broadcast(string eventType, U0 param0, U1 param1, MessengerMode mode)
        {
            MessengerInternal<string>.OnBroadcasting(eventType, mode);

            if (!_eventTable.TryGetValue(eventType, out var d)) return;

            if (d is OnBroadcast<U0, U1> callback)
            {
                callback(param0, param1);
            }
            else
            {
                throw MessengerInternal<string>.CreateBroadcastSignatureException(eventType);
            }
        }
    }

    public static class MessengerString<U0, U1, U2>
    {
        private static Dictionary<string, Delegate> eventTable = MessengerInternal<string>.EventTable;

        public static void AddListener(string eventType, OnBroadcast<U0, U1, U2> handler)
        {
            MessengerInternal<string>.AddListener(eventType, handler);
            eventTable[eventType] = (OnBroadcast<U0, U1, U2>)eventTable[eventType] + handler;
        }

        public static void RemoveListener(string eventType, OnBroadcast<U0, U1, U2> handler)
        {
            MessengerInternal<string>.RemoveListener(eventType, handler);
            eventTable[eventType] = (OnBroadcast<U0, U1, U2>)eventTable[eventType] - handler;
            MessengerInternal<string>.OnRemoveListener(eventType);
        }

        public static void Broadcast(string eventType, U0 param0, U1 param1, U2 param2)
        {
            Broadcast(eventType, param0, param1, param2, MessengerInternal<string>.DEFAULT_MODE);
        }

        private static void Broadcast(string eventType, U0 param0, U1 param1, U2 param2, MessengerMode mode)
        {
            MessengerInternal<string>.OnBroadcasting(eventType, mode);

            if (!eventTable.TryGetValue(eventType, out var d)) return;

            if (d is OnBroadcast<U0, U1, U2> callback)
            {
                callback(param0, param1, param2);
            }
            else
            {
                throw MessengerInternal<string>.CreateBroadcastSignatureException(eventType);
            }
        }
    }
}
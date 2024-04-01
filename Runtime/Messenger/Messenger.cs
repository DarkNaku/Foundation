using System;
using System.Collections.Generic;

namespace DarkNaku.Messenger
{
    public delegate void OnBroadcast();

    public delegate void OnBroadcast<in U0>(U0 param0);

    public delegate void OnBroadcast<in U0, in U1>(U0 param0, U1 param1);

    public delegate void OnBroadcast<in U0, in U1, in U2>(U0 param0, U1 param1, U2 param2);

    public enum MessengerMode
    {
        DONT_REQUIRE_LISTENER,
        REQUIRE_LISTENER,
    }

    internal static class MessengerInternal<T>
    {
        public static Dictionary<T, Delegate> EventTable => _eventTable;
        public static readonly MessengerMode DEFAULT_MODE = MessengerMode.REQUIRE_LISTENER;

        private static Dictionary<T, Delegate> _eventTable = new();

        public static void AddListener(T eventType, Delegate listener)
        {
            EventTable.TryAdd(eventType, null);

            Delegate d = EventTable[eventType];

            if (d != null && d.GetType() != listener.GetType())
            {
                throw new ListenerException(
                    $"[Messenger] Attempting to add listener with inconsistent signature for event type {eventType}. " +
                    $"Current listeners have type {d.GetType().Name} and listener being added has type {listener.GetType().Name}");
            }
        }

        public static void RemoveListener(T eventType, Delegate listener)
        {
            if (EventTable.TryGetValue(eventType, out var d))
            {
                if (d == null)
                {
                    throw new ListenerException(
                        $"[Messenger] Attempting to remove listener with for event type {eventType} but current listener is null.");
                }
                else if (d.GetType() != listener.GetType())
                {
                    throw new ListenerException(
                        $"[Messenger] Attempting to remove listener with inconsistent signature for event type {eventType}." +
                        $"Current listeners have type {d.GetType().Name} and listener being removed has type {listener.GetType().Name}");
                }
            }
            else
            {
                throw new ListenerException(
                    $"[Messenger] Attempting to remove listener for type {eventType} but Messenger doesn't know about this event type.");
            }
        }

        public static void OnRemoveListener(T eventType)
        {
            if (EventTable[eventType] == null)
            {
                EventTable.Remove(eventType);
            }
        }

        public static void OnBroadcasting(T eventType, MessengerMode mode)
        {
            if (mode == MessengerMode.REQUIRE_LISTENER && !EventTable.ContainsKey(eventType))
            {
                throw new BroadcastException($"[Messenger] Broadcasting message {eventType} but no listener found.");
            }
        }

        public static BroadcastException CreateBroadcastSignatureException(T eventType)
        {
            return new BroadcastException(
                $"[Messenger] Broadcasting message {eventType} but listeners have a different signature than the broadcaster.");
        }

        public class BroadcastException : Exception
        {
            public BroadcastException(string msg) : base(msg)
            {
            }
        }

        private class ListenerException : Exception
        {
            public ListenerException(string msg) : base(msg)
            {
            }
        }
    }

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
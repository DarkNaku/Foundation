using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace DarkNaku.Foundation
{
    public struct UnityPriorityQueue<T> : IDisposable where T : struct, IComparable<T> {
        public enum PRIORITY_SORT_TYPE { ASCENDING, DESCENDING };

        private NativeArray<T> _items;
        private HashSet<T> _itemSet; // HashSet for fast contains checks
        private int _count;
        private PRIORITY_SORT_TYPE _sortType;

        public int Count => _count;
        public PRIORITY_SORT_TYPE SortType => _sortType;

        // Constructor
        public UnityPriorityQueue(PRIORITY_SORT_TYPE sortType = PRIORITY_SORT_TYPE.ASCENDING, int capacity = 16, Allocator allocator = Allocator.Persistent) {
            _sortType = sortType;
            _items = new NativeArray<T>(capacity, allocator, NativeArrayOptions.UninitializedMemory);
            _itemSet = new HashSet<T>();
            _count = 0;
        }

        // Add an item to the queue
        public void Add(T item) {
            if (_count == _items.Length) {
                Resize(_items.Length * 2);
            }

            if (_itemSet.Add(item) == false)
            {
                Debug.LogWarning("[UnityPriorityQueue] Add : Item already exists in the queue");
                return;
            }

            _items[_count] = item;
            var index = _count;
            _count++;

            while (index > 0) {
                var parentIndex = (index - 1) / 2;

                if (CompareAndSwap(index, parentIndex)) {
                    index = parentIndex;
                } else {
                    break;
                }
            }
        }

        public bool Contains(T item) => _itemSet.Contains(item);

        public T Pop() {
            if (_count == 0) return default(T);

            T rootItem = _items[0];
            _items[0] = _items[--_count];
            _items[_count] = default(T);

            _itemSet.Remove(rootItem);

            int index = 0;
            while (true) {
                int primaryChildIndex = GetPrimaryChildIndex(index);

                if (primaryChildIndex < 0 || !CompareAndSwap(primaryChildIndex, index)) break;

                index = primaryChildIndex;
            }

            return rootItem;
        }

        public T Peek() => _count > 0 ? _items[0] : default(T);

        private void Resize(int newSize) {
            NativeArray<T> newArray = new NativeArray<T>(newSize, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            NativeArray<T>.Copy(_items, newArray, _count);
            _items.Dispose(); // Dispose the old array to free memory
            _items = newArray;
        }

        // Compare and swap items to maintain heap structure
        private bool CompareAndSwap(int indexA, int indexB) {
            int result = _items[indexA].CompareTo(_items[indexB]);
            bool shouldSwap = (_sortType == PRIORITY_SORT_TYPE.ASCENDING) ? result < 0 : result > 0;

            if (shouldSwap) {
                (_items[indexA], _items[indexB]) = (_items[indexB], _items[indexA]);
            }

            return shouldSwap;
        }

        private int GetPrimaryChildIndex(int parentIndex) {
            int leftChild = (parentIndex * 2) + 1;
            int rightChild = (parentIndex * 2) + 2;

            if (leftChild >= _count) return -1;
            if (rightChild >= _count) return leftChild;

            int comparison = _items[leftChild].CompareTo(_items[rightChild]);

            return (_sortType == PRIORITY_SORT_TYPE.ASCENDING) ?
                (comparison < 0 ? leftChild : rightChild) :
                (comparison > 0 ? leftChild : rightChild);
        }

        public void Dispose() {
            if (_items.IsCreated) {
                _items.Dispose();
            }

            _itemSet.Clear();
        }
    }
}
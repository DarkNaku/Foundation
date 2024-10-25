using System;
using Unity.Collections;
using UnityEngine;

namespace DarkNaku.Foundation
{
    public struct UnityPriorityQueue<T> : IDisposable where T : struct, IComparable<T>
    {
        public enum PRIORITY_SORT_TYPE { ASCENDING, DESCENDING };

        public int Count => _count;
        public PRIORITY_SORT_TYPE SortType { get { return _sortType; } }

        private PRIORITY_SORT_TYPE _sortType;
        private NativeArray<T> _items;
        private int _count;

        public UnityPriorityQueue(PRIORITY_SORT_TYPE sortType = PRIORITY_SORT_TYPE.ASCENDING, int capacity = 16, Allocator allocator = Allocator.Persistent)
        {
            this._sortType = sortType;
            _items = new NativeArray<T>(capacity, allocator, NativeArrayOptions.UninitializedMemory);
            _count = 0;
        }

        public void Add(T item)
        {
            if (_count == _items.Length)
            {
                Resize(_items.Length * 2); // 배열 크기 확장
            }

            _items[_count] = item;
            int index = _count;
            _count++;

            // 힙 구조 유지
            while (index > 0)
            {
                int parentIndex = (index - 1) / 2;
                if (CompareAndSwap(index, parentIndex))
                {
                    index = parentIndex;
                }
                else
                {
                    break;
                }
            }
        }

        public T Pop()
        {
            if (_count == 0) return default(T);

            T rootItem = _items[0];
            _items[0] = _items[--_count];
            _items[_count] = default(T); // 메모리 초기화

            int index = 0;

            while (true)
            {
                int primaryChildIndex = GetPrimaryChildIndex(index);

                if (primaryChildIndex < 0 || !CompareAndSwap(primaryChildIndex, index))
                {
                    break;
                }

                index = primaryChildIndex;
            }

            return rootItem;
        }

        public T Peek()
        {
            return _count > 0 ? _items[0] : default(T);
        }

        private bool CompareAndSwap(int indexA, int indexB)
        {
            int result = _items[indexA].CompareTo(_items[indexB]);
            bool shouldSwap = (_sortType == PRIORITY_SORT_TYPE.ASCENDING) ? result < 0 : result > 0;

            if (shouldSwap)
            {
                (_items[indexA], _items[indexB]) = (_items[indexB], _items[indexA]);
            }

            return shouldSwap;
        }

        private int GetPrimaryChildIndex(int parentIndex)
        {
            int leftChild = (parentIndex * 2) + 1;
            int rightChild = (parentIndex * 2) + 2;

            if (leftChild >= _count) return -1;
            if (rightChild >= _count) return leftChild;

            int comparison = _items[leftChild].CompareTo(_items[rightChild]);

            return (_sortType == PRIORITY_SORT_TYPE.ASCENDING) ?
                (comparison < 0 ? leftChild : rightChild) :
                (comparison > 0 ? leftChild : rightChild);
        }

        private void Resize(int newSize)
        {
            NativeArray<T> newArray = new NativeArray<T>(newSize, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            NativeArray<T>.Copy(_items, newArray, _count);
            _items.Dispose(); // 기존 배열 메모리 해제
            _items = newArray;
        }

        public void Dispose()
        {
            if (_items.IsCreated)
            {
                _items.Dispose(); // 메모리 해제
            }
        }
    }
}
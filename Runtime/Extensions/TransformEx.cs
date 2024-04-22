using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace DarkNaku.Foundation
{
    /// <summary>
    /// Transform의 확장 메서드
    /// </summary>
    public static class TransformEx
    {
        /// <summary>
        /// 자신을 포함한 모든 자식들에게 액션을 실행
        /// </summary>
        /// <param name="transform">부모</param>
        /// <param name="onExecute">실행할 액션</param>
        /// <param name="recursively">재귀적으로 호출하기 위해서는 true로 설정</param>
        public static void ForEachChildIncludeSelf(this Transform transform, [NotNull] Action<Transform> onExecute, bool recursively = false)
        {
            transform.ForEachChildIncludeSelf((child) => 
            {
                onExecute.Invoke(child);
                return true;
            }, recursively);
        }
    
        /// <summary>
        /// 자신을 포함한 모든 자식들에게 액션을 실행하지만 액션의 반환 값이 false일경우 해당 아이템에서 반복을 멈춘다.
        /// </summary>
        /// <param name="transform">부모</param>
        /// <param name="onExecute">실행할 액션</param>
        /// <param name="recursively">재귀적으로 호출하기 위해서는 true로 설정</param>
        public static void ForEachChildIncludeSelf(this Transform transform, [NotNull] Func<Transform, bool> onExecute, bool recusively = false)
        {
            var queue = new Queue<Transform>();
        
            queue.Enqueue(transform);
            
            for (int i = 0; i < transform.childCount; i++)
            {
                queue.Enqueue(transform.GetChild(i));
            }

            while (queue.Count > 0)
            {
                var item = queue.Dequeue();

                if (onExecute.Invoke(item) == false) break;

                if (recusively)
                {
                    for (int i = 0; i < item.childCount; i++)
                    {
                        queue.Enqueue(item.GetChild(i));
                    }
                }
            }
        }
        
        /// <summary>
        /// 자신을 제외한 모든 자식들에게 액션을 실행
        /// </summary>
        /// <param name="transform">부모</param>
        /// <param name="onExecute">실행할 액션</param>
        /// <param name="recursively">재귀적으로 호출하기 위해서는 true로 설정</param>
        public static void ForEachChild(this Transform transform, [NotNull] Action<Transform> onExecute, bool recursively = false)
        {
            transform.ForEachChild((child) => 
            {
                onExecute.Invoke(child);
                return true;
            }, recursively);
        }
    
        /// <summary>
        /// 자신을 제외한 모든 자식들에게 액션을 실행하지만 액션의 반환 값이 false일경우 해당 아이템에서 반복을 멈춘다.
        /// </summary>
        /// <param name="transform">부모</param>
        /// <param name="onExecute">실행할 액션</param>
        /// <param name="recursively">재귀적으로 호출하기 위해서는 true로 설정</param>
        public static void ForEachChild(this Transform transform, [NotNull] Func<Transform, bool> onExecute, bool recusively = false)
        {
            var queue = new Queue<Transform>();
            
            for (int i = 0; i < transform.childCount; i++)
            {
                queue.Enqueue(transform.GetChild(i));
            }
        
            while (queue.Count > 0)
            {
                var item = queue.Dequeue();

                if (onExecute.Invoke(item) == false) break;

                if (recusively)
                {
                    for (int i = 0; i < item.childCount; i++)
                    {
                        queue.Enqueue(item.GetChild(i));
                    }
                }
            }
        }
    }
}
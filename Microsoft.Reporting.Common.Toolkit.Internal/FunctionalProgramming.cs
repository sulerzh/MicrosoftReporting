using System;
using System.Collections.Generic;

namespace Microsoft.Reporting.Common.Toolkit.Internal
{
    internal static class FunctionalProgramming
    {
        internal static IEnumerable<T> TraverseBreadthFirst<T>(T initialNode, Func<T, IEnumerable<T>> getChildNodes, Func<T, bool> traversePredicate)
        {
            Queue<T> queue = new Queue<T>();
            queue.Enqueue(initialNode);
            while (queue.Count > 0)
            {
                T node = queue.Dequeue();
                if (traversePredicate(node))
                {
                    yield return node;
                    IEnumerable<T> childNodes = getChildNodes(node);
                    foreach (T obj in childNodes)
                        queue.Enqueue(obj);
                }
            }
        }
    }
}

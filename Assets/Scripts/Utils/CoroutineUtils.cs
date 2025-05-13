using System;
using System.Collections;
using System.Threading.Tasks;

namespace Utils
{
    public static class CoroutineUtils
    {
        public static IEnumerator AsIEnumerator<T>(this Task<T> task, Action<T> callback)
        {
            while (!task.IsCompleted)
                yield return null;

            if (task.IsFaulted && task.Exception != null)
                throw task.Exception;

            callback(task.Result);
        }
    }
}
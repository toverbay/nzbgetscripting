using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace NzbGetScripting
{
    static class Stopwatch_Extensions
    {
        public static void TimeAction(this Stopwatch sw, Action action, Action<long> beforeAction = null, Action<long, long> afterAction = null)
        {
            sw = sw ?? Stopwatch.StartNew();
            var isRunning = sw.IsRunning;
            if (isRunning)
            {
                sw.Stop();
            }
            var startMs = sw.ElapsedMilliseconds;

            beforeAction?.Invoke(startMs);

            sw.Start();
            action();
            sw.Stop();

            afterAction?.Invoke(sw.ElapsedMilliseconds, sw.ElapsedMilliseconds - startMs);

            if (isRunning)
            {
                sw.Start();
            }
        }

        public static async Task TimeActionAsync(this Stopwatch sw, Task action, Action<long> beforeAction = null, Action<long, long> afterAction = null)
        {
            sw = sw ?? Stopwatch.StartNew();
            var isRunning = sw.IsRunning;
            if (isRunning)
            {
                sw.Stop();
            }
            var startMs = sw.ElapsedMilliseconds;

            beforeAction?.Invoke(startMs);

            sw.Start();
            await action;
            sw.Stop();

            afterAction?.Invoke(sw.ElapsedMilliseconds, sw.ElapsedMilliseconds - startMs);

            if (isRunning)
            {
                sw.Start();
            }
        }

        public static TResult TimeFunction<TResult>(this Stopwatch sw, Func<TResult> action, Action<long> beforeAction = null, Action<TResult, long, long> afterAction = null)
        {
            sw = sw ?? Stopwatch.StartNew();
            var isRunning = sw.IsRunning;
            if (isRunning)
            {
                sw.Stop();
            }
            var startMs = sw.ElapsedMilliseconds;

            beforeAction?.Invoke(startMs);

            sw.Start();
            var result = action();
            sw.Stop();

            afterAction?.Invoke(result, sw.ElapsedMilliseconds, sw.ElapsedMilliseconds - startMs);

            if (isRunning)
            {
                sw.Start();
            }

            return result;
        }

        public static async Task<TResult> TimeFunctionAsync<TResult>(this Stopwatch sw, Func<Task<TResult>> action, Action<long> beforeAction = null, Action<TResult, long, long> afterAction = null)
        {
            sw = sw ?? Stopwatch.StartNew();
            var isRunning = sw.IsRunning;
            if (isRunning)
            {
                sw.Stop();
            }
            var startMs = sw.ElapsedMilliseconds;

            beforeAction?.Invoke(startMs);

            sw.Start();
            var result = await action();
            sw.Stop();

            afterAction?.Invoke(result, sw.ElapsedMilliseconds, sw.ElapsedMilliseconds - startMs);

            if (isRunning)
            {
                sw.Start();
            }

            return result;
        }
    }
}

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace parallel_tasks_exceptions
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await WrapAsync(WaitAll);
            await WrapAsync(WhenAll);

            await WrapWithTaskAsync(WaitAll);
            await WrapWithTaskAsync(WhenAll);

            Log("done!", ConsoleColor.Green);
            Console.ReadLine();
        }

        private static Task WaitAll()
        {
            var t1 = DoSomethingAsync(2, "WaitAll");
            var t2 = DoSomethingAsync(3, "WaitAll");
            Task.WaitAll(t1, t2);

            return Task.CompletedTask;
        }

        private static Task WhenAll()
        {
            var t1 = DoSomethingAsync(1, "WhenAll");
            var t2 = DoSomethingAsync(2, "WhenAll");
            return Task.WhenAll(t1, t2);
        }

        private static async Task WrapAsync(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (AggregateException aggrEx)
            {
                LogException(aggrEx);
            }
            catch (Exception e)
            {
                LogException(e);
            }
        }

        private static async Task WrapWithTaskAsync(Func<Task> action)
        {
            Task t = null;
            try
            {
                t = action();
                await t;
            }
            catch (AggregateException aggrEx)
            {
                LogException(aggrEx);
            }
            catch (Exception e)
            {
                LogException(e);

                if (null != t?.Exception)
                    LogException( t?.Exception );
            }
        }

        private static Task DoSomethingAsync(int seconds, string text)
        {
            return Task.Run(() =>
            {
                Log($"executing {text} for {seconds} seconds...", ConsoleColor.Yellow);
                Thread.Sleep(seconds * 1000);
                throw new InvalidOperationException($"something went wrong executing {text} for {seconds} seconds!");
            });
        }

        private static void LogException(Exception ex)
        {
            if (ex is AggregateException aggrEx) 
            {
                Log($"captured an AggregateException:\n{string.Join("\n", aggrEx.InnerExceptions.Select((ie, index) => $"{index+1}) {ie.Message}") )}", ConsoleColor.Red);
                return;
            }

            Log($"captured an exception of type {ex.GetType().Name} : {ex.Message}", ConsoleColor.Red);
        }

        private static void Log(string text, ConsoleColor color = ConsoleColor.White)
        {
            var oldCol = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = oldCol;
        }
    }
}

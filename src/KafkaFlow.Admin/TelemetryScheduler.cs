namespace KafkaFlow.Admin
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    internal class TelemetryScheduler
    {
       private static readonly Lazy<Dictionary<string, Timer>> timers = new (() => new Dictionary<string, Timer>());

        public static void Set(string clusterName, TimerCallback callback, TimeSpan dueTime, TimeSpan period)
        {
            Unset(clusterName);

            timers.Value[clusterName] = new Timer(callback, null, dueTime, period);
        }

        public static void Unset(string clusterName)
        {
            if (timers.Value.TryGetValue(clusterName, out var timer))
            {
                timer.Dispose();
                timers.Value.Remove(clusterName);
            }
        }
    }
}

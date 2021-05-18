namespace KafkaFlow.Admin.WebApi.Adapters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using KafkaFlow.Admin.Messages;
    using KafkaFlow.Admin.WebApi.Contracts;
    using KafkaFlow.Consumers;
    using Microsoft.Extensions.Caching.Memory;

    internal static class ConsumerResponseAdapter
    {
        internal static ConsumerResponse Adapt(this IMessageConsumer consumer, IMemoryCache cache)
        {
            var consumerResponse = new ConsumerResponse()
            {
                Subscription = consumer.Subscription,
                ConsumerName = consumer.ConsumerName,
                GroupId = consumer.GroupId,
                FlowStatus = consumer.FlowStatus ?? ConsumerFlowStatus.NotRunning,
                MemberId = consumer.MemberId,
                WorkersCount = consumer.WorkersCount,
                ClientInstanceName = consumer.ClientInstanceName,
                IsReadonly = consumer.IsReadonly,
            };
            var key = $"{consumer.GroupId}-{consumer.ConsumerName}";

            var metricCached = cache.TryGetValue(key, out List<ConsumerMetric> metric);

            consumerResponse.PartitionAssignments =
                metricCached ?
                    metric.Select(m => m.Adapt()) :
                    GetLocalInfo(consumer);

            return consumerResponse;
        }

        private static IEnumerable<PartitionAssignment> GetLocalInfo(IMessageConsumer consumer)
        {
            return consumer.PausedPartitions
                .GroupBy(c => c.Topic)
                .Select(c => new PartitionAssignment()
                {
                    Topic = c.Key,
                    HostName = Environment.MachineName,
                    PausedPartitions = c.Select(x => x.Partition.Value).ToList(),
                })
                .Union(consumer.RunningPartitions
                    .GroupBy(c => c.Topic)
                    .Select(c => new PartitionAssignment()
                    {
                        Topic = c.Key,
                        HostName = Environment.MachineName,
                        RunningPartitions = c.Select(x => x.Partition.Value).ToList(),
                    }));
        }

        private static PartitionAssignment Adapt(this ConsumerMetric consumer) =>
            new()
            {
                Topic = consumer.Topic,
                HostName = consumer.HostName,
                PausedPartitions = consumer.PausedPartitions,
                RunningPartitions = consumer.RunningPartitions,
            };
    }
}

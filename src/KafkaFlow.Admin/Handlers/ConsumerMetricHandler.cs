namespace KafkaFlow.Admin.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using KafkaFlow.Admin.Messages;
    using KafkaFlow.TypedHandler;
    using Microsoft.Extensions.Caching.Memory;

    internal class ConsumerMetricHandler : IMessageHandler<ConsumerMetric>
    {
        private readonly IMemoryCache cache;

        public ConsumerMetricHandler(IMemoryCache cache) => this.cache = cache;

        public Task Handle(IMessageContext context, ConsumerMetric message)
        {
            var key = $"{message.GroupId}-{message.ConsumerName}";

            var entry = this.cache.GetOrCreate(key, _ => new List<ConsumerMetric>());

            entry.RemoveAll(e => e.HostName == message.HostName);
            entry.Add(message);
            this.cache.Set(key, entry);

            return Task.CompletedTask;
        }
    }
}

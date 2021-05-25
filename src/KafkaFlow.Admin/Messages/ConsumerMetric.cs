namespace KafkaFlow.Admin.Messages
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// A message that contains data related to consumers partition assigment
    /// </summary>
    [DataContract]
    public class ConsumerMetric : IMetric
    {
        /// <summary>
        /// Gets or sets the consumer group id
        /// </summary>
        [DataMember(Order = 1)]
        public string GroupId { get; set; }

        /// <summary>
        /// Gets or sets the consumer name
        /// </summary>
        [DataMember(Order = 2)]
        public string ConsumerName { get; set; }

        /// <summary>
        /// Gets or sets the topic name
        /// </summary>
        [DataMember(Order = 3)]
        public string Topic { get; set; }

        /// <summary>
        /// Gets or sets the consumer host name
        /// </summary>
        [DataMember(Order = 4)]
        public string HostName { get; set; }

        /// <summary>
        /// Gets or sets the list of running partitions
        /// </summary>
        [DataMember(Order = 5)]
        public IEnumerable<int> RunningPartitions { get; set; }

        /// <summary>
        /// Gets or sets the list of paused partitions
        /// </summary>
        [DataMember(Order = 6)]
        public IEnumerable<int> PausedPartitions { get; set; }

        /// <summary>
        /// Gets or sets the datetime at when the metric was sent
        /// </summary>
        [DataMember(Order = 7)]
        public DateTime SentAt { get; set; }
    }
}

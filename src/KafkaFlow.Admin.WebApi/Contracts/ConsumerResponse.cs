namespace KafkaFlow.Admin.WebApi.Contracts
{
    using System.Collections.Generic;
    using KafkaFlow.Consumers;

    /// <summary>
    /// The response of the consumers
    /// </summary>
    public class ConsumerResponse
    {
       /// <summary>
       /// Gets or sets the consumer´s name
       /// </summary>
       public string ConsumerName { get; set;}

       /// <summary>
       /// Gets or sets the group id
       /// </summary>
       public string GroupId { get; set;}

       /// <summary>
       /// Gets or sets the current number of workers allocated by the consumer
       /// </summary>
       public int WorkersCount { get; set;}

       /// <summary>
       /// Gets the current topics subscription
       /// </summary>
       public IEnumerable<string> Subscription { get; set; }

       /// <summary>
       /// Gets all the consumer partition assignments (data received from metrics events)
       /// </summary>
       public IEnumerable<PartitionAssignment> PartitionAssignments { get; set; }

       /// <summary>
       /// Gets the (dynamic) group member id of this consumer (as set by the broker).
       /// </summary>
       public string MemberId { get; set; }

       /// <summary>
       ///     Gets the name of this client instance.
       ///     Contains (but is not equal to) the client.id configuration parameter.
       /// </summary>
       /// <remarks>
       ///     This name will be unique across all client
       ///     instances in a given application which allows
       ///     log messages to be associated with the
       ///     corresponding instance.
       /// </remarks>
       public string ClientInstanceName { get; set; }

       /// <summary>
       /// Gets the current consumer flow status
       /// </summary>
       public ConsumerFlowStatus FlowStatus { get; set; }
    }
}

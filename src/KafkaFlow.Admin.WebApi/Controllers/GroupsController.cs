namespace KafkaFlow.Admin.WebApi.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using KafkaFlow.Admin.Messages;
    using KafkaFlow.Admin.Producers;
    using KafkaFlow.Admin.WebApi.Adapters;
    using KafkaFlow.Admin.WebApi.Contracts;
    using KafkaFlow.Consumers;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Memory;

    /// <summary>
    /// Groups controller
    /// </summary>
    [Route("kafka-flow/groups")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private readonly IConsumerAccessor consumers;
        private readonly IAdminProducer adminProducer;
        private readonly IMemoryCache cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupsController"/> class.
        /// </summary>
        /// <param name="consumers">The accessor class that provides access to the consumers</param>
        /// <param name="adminProducer">The producer to publish admin messages</param>
        /// <param name="cache">The cache interface to get metric data</param>
        public GroupsController(IConsumerAccessor consumers, IAdminProducer adminProducer, IMemoryCache cache)
        {
            this.consumers = consumers;
            this.adminProducer = adminProducer;
            this.cache = cache;
        }

        /// <summary>
        /// Get all the consumer groups
        /// </summary>
        /// <returns>A list of consumer groups</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<GroupResponse>), 200)]
        public IActionResult Get()
        {
            return this.Ok(
                this.consumers.All
                    .GroupBy(x => x.GroupId)
                    .Select(
                        x => new GroupResponse
                        {
                            GroupId = x.First().GroupId,
                            Consumers = x.Select(y => y.Adapt(this.cache)),
                        }));
        }

        /// <summary>
        /// Pause all consumers from a specific group
        /// </summary>
        /// <param name="groupId">Identifier of the group</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
        [HttpPost]
        [Route("{groupId}/pause")]
        [ProducesResponseType(202)]
        public async Task<IActionResult> Pause([FromRoute] string groupId)
        {
            await this.adminProducer.ProduceAsync(
                new PauseConsumersByGroup
                {
                    GroupId = groupId,
                });

            return this.Accepted();
        }

        /// <summary>
        /// Resume all consumers from a specific group
        /// </summary>
        /// <param name="groupId">Identifier of the group</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
        [HttpPost]
        [Route("{groupId}/resume")]
        [ProducesResponseType(202)]
        public async Task<IActionResult> Resume([FromRoute] string groupId)
        {
            await this.adminProducer.ProduceAsync(
                new ResumeConsumersByGroup
                {
                    GroupId = groupId,
                });

            return this.Accepted();
        }
    }
}

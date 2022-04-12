using KafkaMTWebApp1.Events;
using MassTransit.KafkaIntegration;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace KafkaMTWebApp1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebAppController : ControllerBase
    {
        private readonly ITopicProducer<OrgCreatedEvent> _topicProducer;

        public WebAppController(ITopicProducer<OrgCreatedEvent> topicProducer)
        {
            _topicProducer = topicProducer;
        }

        /// <summary>
        /// Send delete event
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        [HttpPost("{title}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> PostAsync(string title)
        {
            
            await _topicProducer.Produce(new OrgCreatedEvent
            {
                Title = $"{title}"
            });

            return Ok(title);
        }
    }
}
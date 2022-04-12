using KafkaMTWebApp2.Events;
using MassTransit.KafkaIntegration;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace KafkaMTWebApp2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebAppController : ControllerBase
    {
        private readonly ITopicProducer<OrgDeletedEvent> _topicProducer;

        public WebAppController(ITopicProducer<OrgDeletedEvent> topicProducer)
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
            
            await _topicProducer.Produce(new OrgDeletedEvent
            {
                Title = $"{title}"
            });

            return Ok(title);
        }
    }
}
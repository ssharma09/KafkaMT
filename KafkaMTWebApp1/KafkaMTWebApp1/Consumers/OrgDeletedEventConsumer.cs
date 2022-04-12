using KafkaMTWebApp1.Events;
using MassTransit;

namespace KafkaMTWebApp1.Handlers
{
    internal class OrgDeletedEventConsumer : IConsumer<OrgDeletedEvent>
    {
        public Task Consume(ConsumeContext<OrgDeletedEvent> context)
        {
            var message = context.Message.Title;
            return Task.CompletedTask;
        }
    }
}

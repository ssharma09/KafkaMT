using KafkaMTWebApp2.Events;
using MassTransit;

namespace KafkaMTWebApp2.Handlers
{
    internal class OrgCreatedEventConsumer : IConsumer<OrgCreatedEvent>
    {
        public Task Consume(ConsumeContext<OrgCreatedEvent> context)
        {
            var message = context.Message.Title;
            return Task.CompletedTask;
        }
    }
}

using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Scheduling;
using WorkDataMassTransitCourier.Controllers;

namespace WorkDataMassTransitCourier
{
    public class ScheduleNotificationConsumer : IConsumer<ScheduleNotification>
    {
        public Task Consume(ConsumeContext<ScheduleNotification> context)
        {
            Console.WriteLine(context.Message);
            return Task.CompletedTask;
        }
    }

    public class ScheduleNotificationFaultConsumer : IConsumer<Fault<ScheduleNotification>>
    {
 
        public Task Consume(ConsumeContext<Fault<ScheduleNotification>> context)
        {
            throw new NotImplementedException();
        }
    }
}
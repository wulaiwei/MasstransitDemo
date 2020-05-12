using System;
using System.Threading.Tasks;
using MassTransit;
using WorkDataMassTransitSagaPersistence.Saga.Order.Event;

namespace WorkDataMassTransitSagaPersistence.Consumers
{
    public class RemoveStockConsumer: IConsumer<RemoveStock>
    {
        public async Task Consume(ConsumeContext<RemoveStock> context)
        {
            await context.RespondAsync(new RemoveStockResponse
            {
                CompleteTime = DateTime.Now
            });
        }
    }
}
using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Saga;

namespace WorkDataMassTransitSagaPersistence.Saga.Order.Event
{
    public class OrderStateSaga : ISaga, InitiatedBy<SubmitOrder>, Orchestrates<PayOrder>
    {
        public Guid CorrelationId { get; set; }

        public Task Consume(ConsumeContext<PayOrder> context)
        {
            return Task.CompletedTask;
        }
        
        public Task Consume(ConsumeContext<SubmitOrder> context)
        {
            return Task.CompletedTask;
        }
    }
}
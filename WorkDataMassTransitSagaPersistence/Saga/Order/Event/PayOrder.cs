using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Saga;

namespace WorkDataMassTransitSagaPersistence.Saga.Order.Event
{
    public class PayOrder : CorrelatedBy<Guid>
    {
        public PayOrder(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public string OrderId { get; set; }
        public Guid CorrelationId { get; }
    }
}
using System;
using System.Threading.Tasks;
using MassTransit;

namespace WorkDataMassTransitSagaPersistence.Saga.Order.Event
{
    public class SubmitOrder: CorrelatedBy<Guid>
    {
        public SubmitOrder(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public string OrderId { get; set; }
        public Guid CorrelationId { get; }
    }

    public class SubmitOrderLog
    {
        public string Msg { get; set; }
    }
}
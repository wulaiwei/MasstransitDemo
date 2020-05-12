using System;
using MassTransit;

namespace WorkDataMassTransitCourier.StateMachine.Contract
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
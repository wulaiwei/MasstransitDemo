using System;

namespace WorkDataMassTransitCourier.StateMachine.Contract
{
    public class SubmitOrderCompleted
    {
        public Guid OrderId { get; set; }
    }
}
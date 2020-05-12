using System;

namespace WorkDataMassTransitCourier.StateMachine.Contract
{
    public class SubmitOrderFaulted
    {
        public Guid OrderId { get; set; }
    }
}
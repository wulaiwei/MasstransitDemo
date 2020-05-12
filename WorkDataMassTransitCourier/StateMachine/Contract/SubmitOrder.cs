using System;
using MassTransit;

namespace WorkDataMassTransitCourier.StateMachine.Contract
{
    public class SubmitOrder
    {
        public Guid OrderId { get; set; }
        public string ProductKey { get; set; }

        public decimal Quantity { get; set; }
    }

    public class SubmitOrderResponse
    {
        public Guid OrderId { get; set; }
    }

}
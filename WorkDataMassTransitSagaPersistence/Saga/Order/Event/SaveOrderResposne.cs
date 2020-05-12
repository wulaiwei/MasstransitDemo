using System;

namespace WorkDataMassTransitSagaPersistence.Saga.Order.Event
{
    public class SaveOrderResponse
    {
        public DateTime CompleteTime { get; set; }
    }
}
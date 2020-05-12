using System;

namespace WorkDataMassTransitSagaPersistence.Saga.Order.Event
{
    public class RemoveStockResponse
    {
        public DateTime CompleteTime { get; set; }
    }
}
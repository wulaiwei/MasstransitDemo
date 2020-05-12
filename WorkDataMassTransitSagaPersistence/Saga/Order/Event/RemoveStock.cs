using System;

namespace WorkDataMassTransitSagaPersistence.Saga.Order.Event
{
    public class RemoveStock
    {
        public string OrderId { get; set; }
    }
}
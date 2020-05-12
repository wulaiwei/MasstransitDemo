using System;
using Automatonymous;
using MassTransit.EntityFrameworkCoreIntegration.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WorkDataMassTransitSagaPersistence.Saga.Order
{
    public class OrderState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }

        public int CurrentState { get; set; }
        public int ReadyEventStatus { get; set; }
        public string OrderSerialNumber { get; set; }

        // If using Optimistic concurrency, this property is required
        public byte[] RowVersion { get; set; }
    }

    public class OrderStateMap : SagaClassMap<OrderState>
    {
        protected override void Configure(EntityTypeBuilder<OrderState> entity, ModelBuilder model)
        {
            entity.Property(x => x.CurrentState).HasMaxLength(64);
            entity.Property(x => x.OrderSerialNumber).HasMaxLength(200);
            // If using Optimistic concurrency, otherwise remove this property
            entity.Property(x => x.RowVersion).IsRowVersion();
        }
    }
}
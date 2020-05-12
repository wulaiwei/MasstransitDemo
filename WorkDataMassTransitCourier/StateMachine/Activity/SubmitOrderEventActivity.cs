using Automatonymous;
using GreenPipes;
using MassTransit;
using MassTransit.Courier;
using MassTransit.Courier.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkDataMassTransitCourier.StateMachine.Contract;

namespace WorkDataMassTransitCourier.StateMachine.Activity
{
    public class SubmitOrderEventActivity : Automatonymous.Activity<OrderState, SubmitOrder>
    {
        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<OrderState, SubmitOrder> context, Behavior<OrderState, SubmitOrder> next)
        {
            var consumeContext = context.GetPayload<ConsumeContext<SubmitOrder>>();
            var builder = new RoutingSlipBuilder(NewId.NextGuid());
            var orderId = context.Data.OrderId;
            builder.AddActivity("RedisStock", new Uri("queue:redis-stock_execute"), new 
            {
                ProductKey = "ITEM123",
                Quantity = 10.0m
            });

            builder.AddActivity("SaveOrder", new Uri("queue:save-order_execute"), new
            {
                ProductKey = "ITEM123",
                Quantity = 10.0m
            });

            //builder.AddActivity("DbStock", new Uri("queue:db-stock_execute"), new
            //{
            //    ProductKey = "ITEM123",
            //    Quantity = 10.0m
            //});

            builder.AddVariable("OrderId", orderId);

            await builder.AddSubscription(new Uri("queue:saga_order_state"),
                RoutingSlipEvents.Faulted | RoutingSlipEvents.Supplemental,
                RoutingSlipEventContents.None, x => x.Send<SubmitOrderFaulted>(new SubmitOrderFaulted
                {
                    OrderId = orderId
                }));

            await builder.AddSubscription(new Uri("queue:saga_order_state"),
                RoutingSlipEvents.Completed | RoutingSlipEvents.Supplemental,
                RoutingSlipEventContents.None, x => x.Send<SubmitOrderCompleted>(new SubmitOrderCompleted
                {
                    OrderId= orderId
                }));

            var routingSlip = builder.Build();

            await consumeContext.Execute(routingSlip);
            await context.RespondAsync(new SubmitOrderResponse
            {
                OrderId = orderId
            });
            await next.Execute(context);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<OrderState, SubmitOrder, TException> context, Behavior<OrderState, SubmitOrder> next) where TException : Exception
        {
            return next.Faulted(context);
        }

        public void Probe(ProbeContext context)
        {

        }
    }
}

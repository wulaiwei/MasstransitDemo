using System;
using Automatonymous;
using MassTransit;
using WorkDataMassTransitSagaPersistence.Saga.Order.Event;

namespace WorkDataMassTransitSagaPersistence.Saga.Order
{
    public class OrderStateMachine : MassTransitStateMachine<OrderState>
    {
        public OrderStateMachine()
        {
            InstanceState(x => x.CurrentState, Submitted, Pay, Success);
            Event(() => SubmitOrderEvent, x => x.CorrelateById(context => context.Message.CorrelationId));
            Event(() => PayOrderEvent, x => x.CorrelateById(context => context.Message.CorrelationId));
            CompositeEvent(() => SuccessEvent, x => x.ReadyEventStatus, CompositeEventOptions.IncludeInitial, SubmitOrderEvent, PayOrderEvent);

            During(Initial, When(SubmitOrderEvent)
                .Then(x =>
                {
                    var s = x.Instance;
                    Console.WriteLine("xx");
                })
                .Activity(x => 
                    x.OfType<SubmitOrderEventActivity>()
                    )
                .TransitionTo(Submitted)
                .Then(x =>
                {
                    var s = x;
                })
            );

            During(Submitted, When(PayOrderEvent)
                    .Then(x =>
                    {
                        x.Instance.ReadyEventStatus = 2;
                        Console.WriteLine("xx");
                    })
                    .TransitionTo(Pay)
                    )
                ;


            DuringAny(
                When(SuccessEvent)
                    .Then(context =>
                    {
                        var s = 1;
                        Console.WriteLine("Order Ready: {0}", context.Instance.CorrelationId);
                    }));
        }

        public Event<SubmitOrder> SubmitOrderEvent { get; private set; }
        public Event<PayOrder> PayOrderEvent { get; private set; }
        public Automatonymous.Event SuccessEvent { get; private set; }

        public State Submitted { get; set; }
        public State Pay { get; set; }
        public State Success { get; set; }
    }
}
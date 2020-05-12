using System;
using System.Threading.Tasks;
using Automatonymous;
using MassTransit;

namespace WorkDataMassTransitAutomatonymous
{
    public class SubmitOrderConsumer : IConsumer<SubmitOrder>
    {
        public Task Consume(ConsumeContext<SubmitOrder> context)
        {
            return  Task.CompletedTask;
        }
    }
    
    public class SubmitOrder
    {
        public Guid OrderId { get; set; }
    }

    public class PayOrder
    {
        public Guid OrderId { get; set; }
    }

    public class OrderState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public int CurrentState { get; set; }
        public int ReadyEventStatus { get; set; }
    }


    public class OrderStateMachine : MassTransitStateMachine<OrderState>
    {
        public OrderStateMachine()
        {
            InstanceState(x => x.CurrentState, Submitted, Pay, Success);
            Event(() => SubmitOrder, x => x.CorrelateById(context => context.Message.OrderId));
            Event(() => PayOrder, x => x.CorrelateById(context => context.Message.OrderId));

//            Initially(When(SubmitOrder)
//                .Then(x => { Console.WriteLine("xx"); })
//                .TransitionTo(Submitted));
            DuringAny(When(SubmitOrder)
                .Then(x =>
                {
                    x.Instance.ReadyEventStatus = 1;
                    Console.WriteLine("xx");
                })
                .TransitionTo(Submitted));
            During(Submitted, When(PayOrder)
                .Then(x =>
                {
                    x.Instance.ReadyEventStatus = 2;
                    Console.WriteLine("xx");
                })
                .TransitionTo(Pay));

            CompositeEvent(() => SuccessEvent,
                x => x.ReadyEventStatus, SubmitOrder, PayOrder);

            DuringAny(
                When(SuccessEvent)
                    .Then(context =>
                    {
                        var s = 1;
                        Console.WriteLine("Order Ready: {0}", context.Instance.CorrelationId);
                    }).TransitionTo(Success));
        }

        public Event<SubmitOrder> SubmitOrder { get; private set; }
        public Event<PayOrder> PayOrder { get; private set; }
        public Event SuccessEvent { get; private set; }

        public State Submitted { get; set; }
        public State Pay { get; set; }
        public State Success { get; set; }
    }
}
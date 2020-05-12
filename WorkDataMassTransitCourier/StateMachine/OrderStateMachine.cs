using System;
using Automatonymous;
using WorkDataMassTransitCourier.StateMachine.Activity;
using WorkDataMassTransitCourier.StateMachine.Contract;

namespace WorkDataMassTransitCourier.StateMachine
{
    public class OrderStateMachine : MassTransitStateMachine<OrderState>
    {
        public OrderStateMachine()
        {
            InstanceState(x => x.CurrentState, Submitted, Success, Completed, Faulted);
            Event(() => SubmitOrderEvent, x => { x.CorrelateById(w => w.Message.OrderId); });
            Event(() => SubmitOrderCompletedEvent, x => { x.CorrelateById(w => w.Message.OrderId); });
            Event(() => SubmitOrderFaultedEvent, x => { x.CorrelateById(w => w.Message.OrderId); });


            During(Initial, When(SubmitOrderEvent)
                .Then(x =>
                {
                    var s = x.Instance;
                    x.Instance.OrderSerialNumber = Guid.NewGuid().ToString();
                    Console.WriteLine("xx");
                })
                .Activity(x => x.OfType<SubmitOrderEventActivity>())
                .TransitionTo(Submitted)
            );

            DuringAny(When(SubmitOrderFaultedEvent)
                .Then(x =>
                {
                    Console.WriteLine("SubmitOrderFaultedEvent");
                })
                  .TransitionTo(Faulted),
              When(SubmitOrderCompletedEvent)
                .Then(x =>
                {
                    Console.WriteLine("SubmitOrderCompletedEvent");
                })
                  .TransitionTo(Completed));
        }

        public Event<SubmitOrder> SubmitOrderEvent { get; private set; }
        public Event<SubmitOrderFaulted> SubmitOrderFaultedEvent { get; private set; }
        public Event<SubmitOrderCompleted> SubmitOrderCompletedEvent { get; private set; }
        public State Submitted { get; set; }
        public State Success { get; set; }

        public State Faulted { get; private set; }
        public State Completed { get; private set; }
    }
}
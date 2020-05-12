using System;
using System.Threading.Tasks;
using Automatonymous;
using GreenPipes;
using MassTransit;
using MassTransit.Clients;
using MassTransit.Courier.Contracts;
using MassTransit.Courier.Exceptions;

namespace WorkDataMassTransitSagaPersistence.Saga.Order.Event
{
    public class SubmitOrderEventActivity : Activity<OrderState, SubmitOrder>
    {
       private readonly IRequestClient<RemoveStock> _client;
       public SubmitOrderEventActivity(IRequestClient<RemoveStock> client)
       {
           _client = client;
       }

        public void Probe(ProbeContext context)
        {
            
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<OrderState, SubmitOrder> context,
            Behavior<OrderState, SubmitOrder> next)
        {
            throw new RoutingSlipException("123");
            var data = await _client.GetResponse<RemoveStockResponse>(new RemoveStock
            {
                OrderId = context.Data.OrderId
            });
            await context.RespondAsync(new SaveOrderResponse
            {
                CompleteTime = data.Message.CompleteTime
            });
            // call the next activity in the behavior
            await next.Execute(context);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<OrderState, SubmitOrder, TException> context,
            Behavior<OrderState, SubmitOrder> next) where TException : Exception
        {
            Console.Out.WriteLine($"SaveOrderEventActivity:Faulted:{context.Instance.CorrelationId}");
            return next.Faulted(context);
        }
    }
}
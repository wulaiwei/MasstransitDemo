using System;
using System.Threading.Tasks;
using MassTransit.Courier;

namespace WorkDataMassTransitCourier.StateMachine.CourierActivity
{
    public class RedisStockInput
    {
        public Guid OrderId { get; set; }

        public string ProductKey { get; set; }

        public decimal Quantity { get; set; }
    }

    public class RedisStockLog
    {
        public Guid OrderId { get; set; }
        public string Msg { get; set; }
    }


    public class RedisStockActivity : IActivity<RedisStockInput, RedisStockLog>
    {
        public Task<ExecutionResult> Execute(ExecuteContext<RedisStockInput> context)
        {
            var orderId = context.Arguments.OrderId;

            return Task.FromResult(context.Completed(new RedisStockLog { OrderId = context.Arguments.OrderId, Msg = "" }));
        }

        public Task<CompensationResult> Compensate(CompensateContext<RedisStockLog> context)
        {
            return Task.FromResult(context.Compensated());
        }
    }
}
using System;
using System.Threading.Tasks;
using MassTransit.Courier;

namespace WorkDataMassTransitCourier.StateMachine.CourierActivity
{
    public class DbStockInput
    {
        public Guid OrderId { get; set; }

        public string ProductKey { get; set; }

        public decimal Quantity { get; set; }
    }

    public class DbStockStockLog
    {
        public Guid OrderId { get; set; }
        public string Msg { get; set; }
    }

    public class DbStockActivity : IActivity<DbStockInput, DbStockStockLog>
    {
        public Task<ExecutionResult> Execute(ExecuteContext<DbStockInput> context)
        {
            //return Task.FromResult(context.Completed());
            return Task.FromResult(context.Completed(new DbStockStockLog {OrderId = context.Arguments.OrderId}));
        }

        public Task<CompensationResult> Compensate(CompensateContext<DbStockStockLog> context)
        {
            return Task.FromResult(context.Compensated());
        }
    }
}
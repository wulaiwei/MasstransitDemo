using System;
using System.Threading.Tasks;
using MassTransit.Courier;

namespace WorkDataMassTransitCourier.StateMachine.CourierActivity
{
    public class SaveOrderInput
    {
        public Guid OrderId { get; set; }

        public string ProductKey { get; set; }

        public decimal Quantity { get; set; }
    }

    public class SaveOrderStockLog
    {
        public Guid OrderId { get; set; }
        public string Msg { get; set; }
    }

    public class SaveOrderActivity: IActivity<SaveOrderInput,SaveOrderStockLog>
    {
        public Task<ExecutionResult> Execute(ExecuteContext<SaveOrderInput> context)
        {
            var orderId = context.Arguments.OrderId;
            //if (context.Arguments.ProductKey.Contains("ITEM123"))
            //{
            //    throw new Exception("12312");
            //}
            return Task.FromResult(context.Completed(new SaveOrderStockLog {OrderId = context.Arguments.OrderId}));
        }

        public Task<CompensationResult> Compensate(CompensateContext<SaveOrderStockLog> context)
        {
            return Task.FromResult(context.Compensated());
        }
    }
}
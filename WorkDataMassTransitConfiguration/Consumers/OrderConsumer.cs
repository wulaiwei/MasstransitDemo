using System;
using System.Threading.Tasks;
using MassTransit;

namespace WorkDataMassTransitConfiguration.Consumers
{
    public class InsertOrder
    {
        public string ProductName { get; set; }
    }

    public class OrderConsumer : IConsumer<InsertOrder>
    {
        public Task Consume(ConsumeContext<InsertOrder> context)
        {
            Console.Write(context.Message.ProductName);
            throw new Exception("测试错误消息");
            return Task.CompletedTask;
        }
    }

    public class DashboardFaultConsumer :
        IConsumer<Fault<InsertOrder>>
    {
        public async Task Consume(ConsumeContext<Fault<InsertOrder>> context)
        {
            // update the dashboard
            var s = context;
        }
    }
    
    public class InsertOrderObserver : IObserver<ConsumeContext<InsertOrder>>
    {
        public void OnCompleted()
        {
            var b = "1";
        }

        public void OnError(Exception error)
        {
            var s = error;
        }

        public void OnNext(ConsumeContext<InsertOrder> value)
        {
            var s = value;
        }
    }
}
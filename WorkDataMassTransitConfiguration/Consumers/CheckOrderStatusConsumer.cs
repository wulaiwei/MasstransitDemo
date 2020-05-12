using System;
using System.Threading.Tasks;
using MassTransit;

namespace WorkDataMassTransitConfiguration.Consumers
{
    public class CheckOrderStatus
    {
        public string ProductName { get; set; }
    }

    public class CheckOrderStatusResponse
    {
        public string ProductName { get; set; }
    }

    public class CheckOrderStatusConsumer : IConsumer<CheckOrderStatus>
    {
        public async Task Consume(ConsumeContext<CheckOrderStatus> context)
        {
            
            await context.RespondAsync(new CheckOrderStatusResponse
            {
                ProductName = $"{context.Message.ProductName}{DateTime.Now.ToLongDateString()}"
            });
        }
    }
}
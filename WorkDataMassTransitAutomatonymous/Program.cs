using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Saga;

namespace WorkDataMassTransitAutomatonymous
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var machine = new OrderStateMachine();
            var repository = new InMemorySagaRepository<OrderState>();

            var busControl = Bus.Factory.CreateUsingInMemory(cfg =>
            {
                cfg.ReceiveEndpoint("order", e =>
                {
                    e.Consumer<SubmitOrderConsumer>();
                    e.StateMachineSaga(machine, repository);
                });
            });
            try
            {
                do
                {
                    string value = await Task.Run(() =>
                    {
                        Console.WriteLine("Enter message (or quit to exit)");
                        Console.Write("> ");
                        return Console.ReadLine();
                    });

                    if ("quit".Equals(value, StringComparison.OrdinalIgnoreCase))
                        break;

                    var id = Guid.NewGuid();
                    await busControl.Publish(new SubmitOrder
                    {
                        OrderId = id
                    });
                } while (true);
            }
            finally
            {
                await busControl.StopAsync();
            }
        }
    }
}
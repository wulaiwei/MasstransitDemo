using System;
using GreenPipes;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using MassTransit.RabbitMqTransport;
using MassTransit.Saga;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WorkDataMassTransitConfiguration.Consumers;

namespace WorkDataMassTransitConfiguration
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            IBusControl CreateBus(IServiceProvider serviceProvider)
            {
                return Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    cfg.Host(new Uri("rabbitmq://localhost/"), configurator =>
                    {
                        configurator.Username("workdata");
                        configurator.Password("workdata123!@#");
                    });
                    cfg.ReceiveEndpoint("check-order", ep => { ep.ConfigureConsumer<CheckOrderStatusConsumer>(serviceProvider); });
                    cfg.ReceiveEndpoint("insert-order",
                        ep =>
                        {
                            ep.Observer(new InsertOrderObserver(), x => { Console.WriteLine("123"); });
                            ep.ConfigureConsumer<OrderConsumer>(serviceProvider);
                        });

//                    cfg.ReceiveEndpoint("insert-order_error", ep =>
//                    {
//                        ep.ConfigureConsumer<DashboardFaultConsumer>(serviceProvider);
//                    });
                });
            }

            services.AddMassTransit(x =>
            {
                x.AddConsumer<CheckOrderStatusConsumer>();
                x.AddConsumer<OrderConsumer>();
                //x.AddConsumer<DashboardFaultConsumer>();
                x.AddBus(CreateBus);
                x.AddRequestClient<CheckOrderStatus>();
            });

            services.AddMassTransitHostedService();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
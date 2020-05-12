using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Automatonymous;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using MassTransit.EntityFrameworkCoreIntegration.Saga;
using MassTransit.Saga;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WorkDataMassTransitSagaPersistence.Consumers;
using WorkDataMassTransitSagaPersistence.Saga;
using WorkDataMassTransitSagaPersistence.Saga.Order;
using WorkDataMassTransitSagaPersistence.Saga.Order.Event;


namespace WorkDataMassTransitSagaPersistence
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
            var connectionString =
                "Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=SagaInstance0402;Integrated Security=True;";

            IBusControl CreateBus(IServiceProvider serviceProvider)
            {
                return Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    cfg.Host(new Uri("rabbitmq://localhost/"), configurator =>
                    {
                        configurator.Username("workdata");
                        configurator.Password("workdata123!@#");
                    });
                  
                    cfg.ReceiveEndpoint("saga_order_state",
                        ep =>
                        {
                            ep.Saga<OrderState>(serviceProvider.GetRequiredService<ISagaRepository<OrderState>>());
                            ep.StateMachineSaga(
                                serviceProvider.GetRequiredService<OrderStateMachine>(),
                                serviceProvider.GetRequiredService<ISagaRepository<OrderState>>(), configurator =>
                            {
                            });
                        });

                    cfg.ReceiveEndpoint("remove_stock",
                        ep => { 
                            ep.ConfigureConsumer<RemoveStockConsumer>(serviceProvider);
                        });
                });
            }

            services.AddMassTransit(cfg =>
            {
                cfg.AddConsumer<RemoveStockConsumer>();

                cfg.AddRequestClient<SubmitOrder>();
                cfg.AddRequestClient<RemoveStock>();
                cfg.AddSagaStateMachine<OrderStateMachine, OrderState>()
                    .EntityFrameworkRepository(r =>
                    {
                        r.ConcurrencyMode = ConcurrencyMode.Pessimistic; // or use Optimistic, which requires RowVersion

                        r.AddDbContext<DbContext, OrderStateDbContext>((provider, builder) =>
                        {
                            builder.UseSqlServer(connectionString, m =>
                            {
                                m.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                                m.MigrationsHistoryTable($"__{nameof(OrderStateDbContext)}");
                            });
                        });
                    });
                cfg.AddBus(CreateBus);
            })
                .AddScoped<SubmitOrderEventActivity>();
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
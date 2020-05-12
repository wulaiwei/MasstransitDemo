using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.SqlServer;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using MassTransit.HangfireIntegration;
using MassTransit.Saga;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WorkDataMassTransitCourier.StateMachine;
using WorkDataMassTransitCourier.StateMachine.Activity;
using WorkDataMassTransitCourier.StateMachine.Contract;
using WorkDataMassTransitCourier.StateMachine.CourierActivity;

namespace WorkDataMassTransitCourier
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        /// ConfigureServices
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            var connectionString =
                "Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=SagaInstanceHangfire;Integrated Security=True;";

            services.AddHangfire((provider, configuration) => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
                {
                    QueuePollInterval = TimeSpan.FromSeconds(15),
                    JobExpirationCheckInterval = TimeSpan.FromHours(1),
                    CountersAggregateInterval = TimeSpan.FromMinutes(5),
                    PrepareSchemaIfNecessary = true,
                    DashboardJobListLimit = 50000,
                    TransactionTimeout = TimeSpan.FromMinutes(1)
                }));

            //Hangfire
            services.AddHangfireServer();

            services.AddTransient<IHangfireComponentResolver, ServiceProviderHangfireComponentResolver>();

            IBusControl CreateBus(IServiceProvider serviceProvider)
            {
                return Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    cfg.Host(new Uri("rabbitmq://localhost/"), configurator =>
                    {
                        configurator.Username("workdata");
                        configurator.Password("workdata123!@#");
                    });

                    cfg.UseHangfireScheduler(
                        serviceProvider.GetService<IHangfireComponentResolver>(),
                        "hangfire", options =>
                        {
                            /*configure background server*/
            
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
                    cfg.ReceiveEndpoint("redis-stock_execute", action =>
                    {
                        action.ExecuteActivityHost<RedisStockActivity, RedisStockInput>(new Uri("queue:redis-stock_execute_error"));
                    });
                    cfg.ReceiveEndpoint("redis-stock_execute_error", action =>
                    {
                        action.CompensateActivityHost<RedisStockActivity, RedisStockLog>();
                    });
                    cfg.ReceiveEndpoint("save-order_execute", action =>
                    {
                        action.ExecuteActivityHost<SaveOrderActivity, SaveOrderInput>(new Uri("queue:save-order_execute_error"));
           
                    });
                    cfg.ReceiveEndpoint("save-order_execute_error", action =>
                    {
                        action.CompensateActivityHost<SaveOrderActivity, SaveOrderStockLog>();
                    });
                    cfg.ReceiveEndpoint("schedule_test_queue", e =>
                    {
                        e.Consumer<ScheduleNotificationConsumer>();
                    });
                });
            }

            services.AddMassTransit(cfg =>
            {
                cfg.AddRequestClient<SubmitOrder>();
                cfg.AddConsumer<ScheduleNotificationConsumer>();
                cfg.AddActivitiesFromNamespaceContaining<RedisStockActivity>();
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

        /// <summary>
        /// Configure
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseHangfireDashboard();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using WorkDataMassTransitSagaPersistence.Saga.Order;

namespace WorkDataMassTransitSagaPersistence.Saga
{
    public class ContextFactory: IDesignTimeDbContextFactory<OrderStateDbContext>
    {
        public OrderStateDbContext CreateDbContext(string[] args)
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<OrderStateDbContext>();

            dbContextOptionsBuilder.UseSqlServer("Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=SagaInstance0402;Integrated Security=True;",
                m =>
                {
                    var executingAssembly = typeof(ContextFactory).GetTypeInfo().Assembly;
                    m.MigrationsAssembly(executingAssembly.GetName().Name);
                });

            return new OrderStateDbContext(dbContextOptionsBuilder.Options);
        }
    }
}
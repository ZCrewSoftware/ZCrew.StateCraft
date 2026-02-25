using System.Reflection;
using Microsoft.EntityFrameworkCore;
using OrderProcessor.Domain.Models;

namespace OrderProcessor.Infrastructure;

public class OrderProcessorDbContext : DbContext
{
    public required DbSet<Order> Orders { get; set; }

    public OrderProcessorDbContext(DbContextOptions<OrderProcessorDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OrderProcessor.Domain.Models;

namespace OrderProcessor.Infrastructure.Configuration;

public class OrderEntityConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.State).HasConversion<EnumToStringConverter<OrderState>>();

        builder.Navigation(x => x.Lines).AutoInclude();

        builder.HasMany(x => x.Lines).WithOne().OnDelete(DeleteBehavior.Cascade);
    }
}

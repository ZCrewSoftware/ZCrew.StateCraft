using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OrderProcessor.Domain.Models;

namespace OrderProcessor.Infrastructure.Configuration;

public class LineEntityConfiguration : IEntityTypeConfiguration<Line>
{
    public void Configure(EntityTypeBuilder<Line> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.State).HasConversion<EnumToStringConverter<LineState>>();
    }
}

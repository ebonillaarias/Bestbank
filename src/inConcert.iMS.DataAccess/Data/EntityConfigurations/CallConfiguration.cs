using inConcert.iMS.Domain.Entities;
using inConcert.iMS.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace inConcert.iMS.DataAccess.Data.EntityConfigurations
{
   public class CallConfiguration : IEntityTypeConfiguration<Calls>
    {
        public void Configure(EntityTypeBuilder<Calls> builder)
        {
            builder.ToTable(nameof(Calls));

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            builder.Property(p => p.CallerId).IsRequired().HasMaxLength(20);
            builder.Property(p => p.CalledId).IsRequired().HasMaxLength(20);
            builder.Property(p => p.Direction).IsRequired().HasConversion(
                v => v.ToString(),
                v => (CallDirection)Enum.Parse(typeof(CallDirection), v));
            builder.Property(p => p.StartDate).IsRequired().HasDefaultValue(DateTimeOffset.UtcNow);
            builder.Property(p => p.EndDate).HasDefaultValue(null);
            builder.Property(p => p.CustomerId).HasDefaultValue("");
            builder.Property(p => p.CustomerName).HasDefaultValue("");
        }
    }
}

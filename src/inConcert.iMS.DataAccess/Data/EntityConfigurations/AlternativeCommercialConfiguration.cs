using inConcert.iMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace inConcert.iMS.DataAccess.Data.EntityConfigurations
{
    [ExcludeFromCodeCoverage]
    public class AlternativeCommercialConfiguration : IEntityTypeConfiguration<AlternativeCommercials>
    {
        public void Configure(EntityTypeBuilder<AlternativeCommercials> builder)
        {
            builder.ToTable(nameof(AlternativeCommercials));

            builder.HasKey(ac => new { ac.CommercialId, ac.AlternativeCommercialId });

            builder.HasOne(ac => ac.Commercial).WithMany(c => c.AlternativeCommercials)
                .HasForeignKey(ac => ac.CommercialId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(ac => ac.AlternativeCommercialProp).WithOne(c => c.AlternativeCommercial)
                .HasForeignKey<AlternativeCommercials>(ac => ac.AlternativeCommercialId).OnDelete(DeleteBehavior.Restrict);

            builder.Property(p => p.Order).IsRequired().HasMaxLength(11);
        }
    }
}

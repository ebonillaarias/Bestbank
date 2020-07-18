using inConcert.iMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace inConcert.iMS.DataAccess.Data.EntityConfigurations
{
   public class SessionConfiguration : IEntityTypeConfiguration<Sessions>
   {
      public void Configure(EntityTypeBuilder<Sessions> builder)
      {
         builder.ToTable(nameof(Sessions));

         builder.HasKey(s => s.Id);
         builder.Property(p => p.Id).ValueGeneratedOnAdd();

         builder.HasOne(s => s.Comercial).WithOne(c => c.Session).HasForeignKey<Sessions>(s => s.CommercialId)
             .OnDelete(DeleteBehavior.Restrict);

         builder.Property(p => p.StartDate).IsRequired().HasDefaultValue(DateTimeOffset.UtcNow);
         builder.Property(p => p.LastKeepAlive).HasDefaultValue(null);
         builder.Property(p => p.EndDate).HasDefaultValue(null);
         builder.Property(p => p.IdExternal);
         builder.Property(p => p.FirebaseToken);
      }
   }
}

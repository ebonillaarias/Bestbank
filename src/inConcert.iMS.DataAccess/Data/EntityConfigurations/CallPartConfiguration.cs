using inConcert.iMS.Domain.Entities;
using inConcert.iMS.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace inConcert.iMS.DataAccess.Data.EntityConfigurations
{
   public class CallPartConfiguration : IEntityTypeConfiguration<CallParts>
   {
      public void Configure(EntityTypeBuilder<CallParts> builder)
      {
         builder.ToTable(nameof(CallParts));

         builder.HasKey(cp => cp.Id);
         builder.Property(cp => cp.Id).ValueGeneratedOnAdd();

         builder.HasOne(cp => cp.Call).WithMany(c => c.CallParts).HasForeignKey(cp => cp.CallId).OnDelete(DeleteBehavior.Restrict);

         builder.Property(cp => cp.CallPartNumber).IsRequired().HasMaxLength(20);
         builder.Property(cp => cp.Peer).IsRequired().HasMaxLength(4);
         builder.Property(cp => cp.CommercialId).IsRequired();
         builder.Property(cp => cp.CommercialName).IsRequired();
         builder.Property(cp => cp.OrigChannel).IsRequired().HasMaxLength(100);
         builder.Property(cp => cp.RedirectChannel).IsRequired().HasMaxLength(100);
         builder.Property(cp => cp.StartDate).IsRequired().HasDefaultValue(DateTimeOffset.UtcNow);
         builder.Property(cp => cp.EndDate).HasDefaultValue(null);
         builder.Property(p => p.CallResult).HasConversion(
             v => v.ToString(),
             v => (CallResult)Enum.Parse(typeof(CallResult), v)).HasDefaultValue(CallResult.InProgress);
         builder.Property(p => p.CallPartEndedBy).HasConversion(
             v => v.ToString(),
             v => (CallPartEndedBy)Enum.Parse(typeof(CallPartEndedBy), v)).HasDefaultValue(CallPartEndedBy.Undefined);
         builder.Property(cp => cp.FilePath).IsRequired(false);
         builder.Property(cp => cp.RejectionReason);
      }
   }
}

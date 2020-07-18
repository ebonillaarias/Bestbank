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
   public class CommercialConfiguration : IEntityTypeConfiguration<Commercials>
   {
      public void Configure(EntityTypeBuilder<Commercials> builder)
      {
         builder.ToTable(nameof(Commercials));

         builder.HasKey(p => new { p.Id });
         builder.Property(p => p.Id).ValueGeneratedOnAdd();

         builder.Property(c => c.Email).IsRequired();
         builder.HasIndex(c => c.Email).IsUnique();

         builder.Property(p => p.Password).IsRequired().HasMaxLength(255);
         builder.Property(p => p.Name).IsRequired().HasMaxLength(255);
         
         builder.Property(p => p.Peer).IsRequired().HasMaxLength(4);
         builder.HasIndex(p => p.Peer).IsUnique();

         builder.Property(p => p.SiebelId).IsRequired().HasMaxLength(15);
         builder.HasIndex(p => p.SiebelId).IsUnique();

         builder.Property(p => p.PBXPhoneNumber).IsRequired().HasMaxLength(20);
         builder.HasIndex(p => p.PBXPhoneNumber).IsUnique();
         
         builder.Property(p => p.MobilePhoneNumber).IsRequired().HasMaxLength(20);
         builder.HasIndex(p => p.MobilePhoneNumber).IsUnique();
         
         builder.Property(p => p.Active).IsRequired().HasDefaultValue(false);
         builder.Property(p => p.PasswordChangeRequired).IsRequired().HasDefaultValue(false);
      }
   }
}

using inConcert.iMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace inConcert.iMS.DataAccess.Data.EntityConfigurations
{
    public class LogsGeneralsConfiguration : IEntityTypeConfiguration<LogsGenerals>
    {
        public void Configure(EntityTypeBuilder<LogsGenerals> builder)
        {
            builder.ToTable(nameof(LogsGenerals));

            builder.HasKey(s => s.Id);
            builder.Property(p => p.Id).ValueGeneratedOnAdd();



            //builder.HasOne(s => s.Comercial).WithOne(c => c.LogsGeneral).HasForeignKey<LogsGenerals>(s => s.CommercialId)
            // .OnDelete(DeleteBehavior.Restrict);

            //builder.HasOne(s => s.Call).WithOne(c => c.LogsGeneral).HasForeignKey<LogsGenerals>(s => s.CallId)
            //.OnDelete(DeleteBehavior.Restrict);

            //builder.HasOne(s => s.Supervisors).WithOne(c => c.LogsGeneral).HasForeignKey<LogsGenerals>(s => s.SupervisorsId)
            //.OnDelete(DeleteBehavior.Restrict);

            //builder.HasOne(s => s.Supervisors).WithOne(c => c.LogsGeneral).HasForeignKey<LogsGenerals>(s => s.SupervisorsEmail)
            //.OnDelete(DeleteBehavior.Restrict);

            builder.Property(p => p.TypeLog).HasDefaultValue(null);
            builder.Property(p => p.Description).HasDefaultValue(null);
            builder.Property(p => p.HourLog).IsRequired().HasDefaultValue(DateTimeOffset.UtcNow);                        
            builder.Property(p => p.UserId).IsRequired().HasDefaultValue(1173);
            builder.Property(p => p.CallsId).IsRequired().HasDefaultValue(6564);
            //builder.Property(p => p.SupervisorsId).IsRequired().HasDefaultValue(1173);
            //builder.Property(p => p.SupervisorsEmail).IsRequired().HasDefaultValue(6564);

        }
    }
}
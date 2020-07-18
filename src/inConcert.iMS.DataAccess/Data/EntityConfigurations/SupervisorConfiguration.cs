using inConcert.iMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace inConcert.iMS.DataAccess.Data.EntityConfigurations
{
    public class SupervisorConfiguration : IEntityTypeConfiguration<Supervisors>
    {
        public void Configure(EntityTypeBuilder<Supervisors> builder)
        {
            builder.ToTable(nameof(Supervisors));

            builder.HasKey(s => new { s.Id, s.Email });
            builder.HasIndex(s => s.Email).IsUnique();
            builder.Property(s => s.Id).ValueGeneratedOnAdd();

            builder.Property(s => s.Password).IsRequired().HasMaxLength(255);
            builder.Property(s => s.Name).IsRequired().HasMaxLength(255);
            builder.Property(s => s.State).IsRequired();
            
        }
    }
}

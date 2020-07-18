using inConcert.iMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace inConcert.iMS.DataAccess.Data.EntityConfigurations
{
    public class CallCustomerConfiguration : IEntityTypeConfiguration<CallsCustomers>
    {
        public void Configure(EntityTypeBuilder<CallsCustomers> builder)
        {
            builder.ToTable(nameof(CallsCustomers));

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedOnAdd();
            builder.Property(s => s.CallId);
            builder.Property(s => s.CustomerId).IsRequired();
            builder.Property(s => s.CustomerName).IsRequired();
            builder.Property(s => s.CustomerType).IsRequired();
        }
    }
}
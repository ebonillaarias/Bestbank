using inConcert.iMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace inConcert.iMS.DataAccess.Data.EntityConfigurations
{
    public class CustomerPhoneConfiguration : IEntityTypeConfiguration<CustomersPhones>
    {
        public void Configure(EntityTypeBuilder<CustomersPhones> builder)
        {
            builder.ToTable(nameof(CustomersPhones));

            builder.HasKey(s => new { s.Id} );
            builder.Property(s => s.Id).ValueGeneratedOnAdd();
            builder.Property(s => s.PhoneNumber).IsRequired();
            builder.Property(s => s.PhoneType).IsRequired();
            builder.Property(s => s.CustomerId).IsRequired();
        }
    }
}

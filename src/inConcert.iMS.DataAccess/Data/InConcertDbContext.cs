using System.Diagnostics.CodeAnalysis;
using inConcert.iMS.DataAccess.Data.EntityConfigurations;
using inConcert.iMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace inConcert.iMS.DataAccess.Data
{
   [ExcludeFromCodeCoverage]
   public class InConcertDbContext : DbContext
   {
      private readonly string _schema;

      public DbSet<Commercials> Commercials { get; set; }
      public DbSet<AlternativeCommercials> AlternativeCommercials { get; set; }
      public DbSet<Sessions> Sessions { get; set; }
      public DbSet<Calls> Calls { get; set; }
      public DbSet<CallParts> CallParts { get; set; }
      public DbSet<Supervisors> Supervisors { get; set; }
      public DbSet<CustomersPhones> CustomersPhones { get; set; }
      public DbSet<CallsCustomers> CallsCustomers { get; set; }
      public DbSet<LogsGenerals> LogsGenerals { get; set; }

        public InConcertDbContext(DbContextOptions<InConcertDbContext> options)
        : base(options)
      {
      }
      public InConcertDbContext(DbContextOptions<InConcertDbContext> options, string schema)
          : base(options)
      {
         _schema = schema;
      }

      protected override void OnModelCreating(ModelBuilder modelBuilder)
      {
         base.OnModelCreating(modelBuilder);

         if (!string.IsNullOrWhiteSpace(_schema))
            modelBuilder.HasDefaultSchema(_schema);

         modelBuilder.ApplyConfiguration(new CommercialConfiguration());
         modelBuilder.ApplyConfiguration(new AlternativeCommercialConfiguration());
         modelBuilder.ApplyConfiguration(new CallConfiguration());
         modelBuilder.ApplyConfiguration(new CallPartConfiguration());
         modelBuilder.ApplyConfiguration(new SessionConfiguration());
         modelBuilder.ApplyConfiguration(new SupervisorConfiguration());
         modelBuilder.ApplyConfiguration(new CustomerPhoneConfiguration());
         modelBuilder.ApplyConfiguration(new CallCustomerConfiguration());
         modelBuilder.ApplyConfiguration(new LogsGeneralsConfiguration());
        }
   }
}

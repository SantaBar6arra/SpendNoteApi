using Data.Models;
using System.Data.Entity;

namespace Data.Context
{
    public class SpendNoteContext : DbContext
    {
        public SpendNoteContext()
            : base("data source=RAPID-HUNTER; initial catalog=SpendNote; integrated security=SSPI") // connection string when doing new migrations
        {

        }

        public SpendNoteContext(string nameOrConnectionString) : 
            base(nameOrConnectionString) 
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
               .HasOptional(p => p.List)
               .WithMany()
               .WillCascadeOnDelete(true);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Income> Incomes { get; set; }
        public DbSet<IncomeCategory> IncomeCategories { get; set; }
        public DbSet<List> Lists { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<VerificationCode> VerificationCodes { get; set; }
    }
}

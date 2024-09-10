using System; // Importing the System namespace for basic functionalities.
using System.Collections.Generic; // Importing the namespace for using generic collections like List.
using System.Linq; // Importing the namespace for LINQ functionalities.
using System.Threading.Tasks; // Importing the namespace for asynchronous programming.
using api.Models; // Importing the namespace where the application's models are defined.
using Microsoft.AspNetCore.Identity; // Importing the namespace for ASP.NET Core Identity functionalities.
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // Importing the namespace for IdentityDbContext.
using Microsoft.EntityFrameworkCore; // Importing the namespace for Entity Framework Core functionalities.

namespace api.Data // Defining the namespace for the data context.
{
    // Defining the ApplicationDBContext class which inherits from IdentityDbContext with AppUser as the user entity.
    public class ApplicationDBContext : IdentityDbContext<AppUser>
    {
        // Constructor for ApplicationDBContext which takes DbContextOptions and passes it to the base class constructor.
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> DbContextOptions) : base(DbContextOptions)
        {
            
        }

        // Defining a DbSet for Stock entities.
        public DbSet<Stock> Stocks { get; set; }
        // Defining a DbSet for Comment entities.
        public DbSet<Comment> Comments { get; set; }
        // Defining a DbSet for Portfolio entities.
        public DbSet<Portfolio> Portfolios { get; set; }

        // Overriding the OnModelCreating method to configure the model.
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Calling the base class's OnModelCreating method.
            base.OnModelCreating(builder);

            // Configuring the Portfolio entity to have a composite key consisting of AppUserId and StockId.
            builder.Entity<Portfolio>(x => x.HasKey(p => new { p.AppUserId, p.StockId }));

            // Configuring the relationship between Portfolio and AppUser entities.
            builder.Entity<Portfolio>()
            .HasOne(u => u.AppUser) // A Portfolio has one AppUser.
            .WithMany(u => u.Portfolios) // An AppUser can have many Portfolios.
            .HasForeignKey(p => p.AppUserId); // The foreign key is AppUserId.

            // Configuring the relationship between Portfolio and Stock entities.
            builder.Entity<Portfolio>()
            .HasOne(u => u.Stock) // A Portfolio has one Stock.
            .WithMany(u => u.Portfolios) // A Stock can have many Portfolios.
            .HasForeignKey(p => p.StockId); // The foreign key is StockId.

            // Creating a list of IdentityRole objects to seed the database with roles.
            List<IdentityRole> roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Name = "Admin", // Defining the Admin role.
                    NormalizedName="ADMIN" // Normalized name for the Admin role.
                },
                new IdentityRole
                {
                    Name = "User", // Defining the User role.
                    NormalizedName="USER" // Normalized name for the User role.
                },
            };

            // Seeding the roles into the database.
            builder.Entity<IdentityRole>().HasData(roles);
        }
    }
}
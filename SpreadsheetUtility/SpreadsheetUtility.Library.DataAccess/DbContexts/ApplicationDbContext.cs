using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SpreadsheetUtility.Library.DataAccess.Models;

namespace SpreadsheetUtility.Library.DataAccess.DbContexts;

/// <summary>
/// Application database context for the spreadsheet utility.
/// Inherits from IdentityDbContext to support user authentication and authorization.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class.
    /// </summary>
    /// <param name="options">The database context options.</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the DbSet for ApplicationUser entities.
    /// </summary>
    public override DbSet<ApplicationUser> Users { get; set; } = null!;

    /// <summary>
    /// Configures the database model during the model creation process.
    /// </summary>
    /// <param name="modelBuilder">The model builder used to construct the database model.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure ApplicationUser
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("Users");

            entity.Property(u => u.FirstName)
                .HasMaxLength(100)
                .IsRequired(false);

            entity.Property(u => u.LastName)
                .HasMaxLength(100)
                .IsRequired(false);

            entity.Property(u => u.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(u => u.IsActive)
                .HasDefaultValue(true);
        });

        // Configure Identity Role
        modelBuilder.Entity<IdentityRole<int>>(entity =>
        {
            entity.ToTable("Roles");
        });

        // Configure Identity User Role
        modelBuilder.Entity<IdentityUserRole<int>>(entity =>
        {
            entity.ToTable("UserRoles");
        });

        // Configure Identity User Claim
        modelBuilder.Entity<IdentityUserClaim<int>>(entity =>
        {
            entity.ToTable("UserClaims");
        });

        // Configure Identity Role Claim
        modelBuilder.Entity<IdentityRoleClaim<int>>(entity =>
        {
            entity.ToTable("RoleClaims");
        });

        // Configure Identity User Login
        modelBuilder.Entity<IdentityUserLogin<int>>(entity =>
        {
            entity.ToTable("UserLogins");
        });

        // Configure Identity User Token
        modelBuilder.Entity<IdentityUserToken<int>>(entity =>
        {
            entity.ToTable("UserTokens");
        });
    }
}

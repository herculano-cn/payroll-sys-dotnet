using Microsoft.EntityFrameworkCore;
using Payroll.Domain.Entities;

namespace Payroll.Infrastructure.Data;

/// <summary>
/// Entity Framework Core DbContext for Payroll system.
/// Maps to COBOL: FOLHA1 file (indexed sequential file)
/// </summary>
public class PayrollDbContext : DbContext
{
    public PayrollDbContext(DbContextOptions<PayrollDbContext> options) 
        : base(options)
    {
    }

    /// <summary>
    /// Employees DbSet
    /// Maps to COBOL: FOLHA1 file with FS-COLABORADOR records
    /// </summary>
    public DbSet<Employee> Employees { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Employee entity
        modelBuilder.Entity<Employee>(entity =>
        {
            // Table name
            entity.ToTable("Employees");

            // Primary key
            entity.HasKey(e => e.Id);

            // Indexes for performance
            // Maps to COBOL: ORGANIZATION IS INDEXED with RECORD KEY IS FS-CHAVE
            entity.HasIndex(e => e.EmployeeId)
                .IsUnique()
                .HasDatabaseName("IX_Employees_EmployeeId");

            entity.HasIndex(e => new { e.ReferenceMonth, e.ReferenceYear })
                .HasDatabaseName("IX_Employees_ReferencePeriod");

            entity.HasIndex(e => e.IsDeleted)
                .HasDatabaseName("IX_Employees_IsDeleted");

            // String properties with max length
            entity.Property(e => e.EmployeeId)
                .IsRequired()
                .HasMaxLength(5);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(35);

            entity.Property(e => e.Position)
                .IsRequired()
                .HasMaxLength(30);

            entity.Property(e => e.CNPJ)
                .IsRequired()
                .HasMaxLength(14);

            // Decimal properties with precision
            // CRITICAL: Use decimal(18,2) for all money fields
            entity.Property(e => e.BaseSalary)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entity.Property(e => e.GrossSalary)
                .HasColumnType("decimal(18,2)");

            entity.Property(e => e.NetSalary)
                .HasColumnType("decimal(18,2)");

            entity.Property(e => e.TotalOvertime)
                .HasColumnType("decimal(18,2)");

            entity.Property(e => e.WeeklyRest)
                .HasColumnType("decimal(18,2)");

            entity.Property(e => e.INSS)
                .HasColumnType("decimal(18,2)");

            entity.Property(e => e.IRRF)
                .HasColumnType("decimal(18,2)");

            entity.Property(e => e.FamilyAllowance)
                .HasColumnType("decimal(18,2)");

            entity.Property(e => e.DependentDeduction)
                .HasColumnType("decimal(18,2)");

            entity.Property(e => e.TransportationVoucher)
                .HasColumnType("decimal(18,2)");

            entity.Property(e => e.AbsenceDeduction)
                .HasColumnType("decimal(18,2)");

            entity.Property(e => e.FGTS)
                .HasColumnType("decimal(18,2)");

            // Date properties
            entity.Property(e => e.HireDate)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .IsRequired();

            entity.Property(e => e.UpdatedAt)
                .IsRequired();

            // Soft delete
            entity.Property(e => e.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            // Query filter for soft delete (automatically excludes deleted records)
            entity.HasQueryFilter(e => !e.IsDeleted);
        });
    }

    /// <summary>
    /// Override SaveChanges to automatically update timestamps
    /// </summary>
    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    /// <summary>
    /// Override SaveChangesAsync to automatically update timestamps
    /// </summary>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Automatically updates CreatedAt and UpdatedAt timestamps
    /// </summary>
    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries<Employee>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}
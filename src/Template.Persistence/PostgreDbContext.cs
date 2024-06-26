using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Template.Domain.Entities.Identity;
using Template.Persistence.EntitiesConfigurations.Identity;
using Template.Domain.Entities;
using Template.Domain.Entities.Audit;
using Template.Persistence.Common;
using Template.Persistence.Models;

namespace Template.Persistence;

public class PostgreDbContext : IdentityDbContext<User, Role, long, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
{
	private readonly IAuditUserProvider? _auditUserProvider;

	public PostgreDbContext(DbContextOptions options, IAuditUserProvider auditUserProvider) : base(options)
	{
		_auditUserProvider = auditUserProvider;
	}

	public PostgreDbContext(DbContextOptions options) : base(options)
	{
	}


	public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
	public DbSet<Audit> Audits => Set<Audit>();
	public DbSet<AuditMetaData> AuditMetaData => Set<AuditMetaData>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);
		modelBuilder.ApplyConfiguration(new RoleClaimConfiguration());
		modelBuilder.ApplyConfiguration(new RoleConfiguration());
		modelBuilder.ApplyConfiguration(new UserConfiguration());
		modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
		modelBuilder.ApplyConfiguration(new UserClaimConfiguration());
		modelBuilder.ApplyConfiguration(new UserLoginConfiguration());
		modelBuilder.ApplyConfiguration(new UserTokenConfiguration());

		modelBuilder.Entity<User>().HasMany(s => s.Roles)
			.WithMany(c => c.Users)
			.UsingEntity<UserRole>(
				l => l.HasOne<Role>(e => e.Role).WithMany().HasForeignKey(e => e.RoleId),
				r => r.HasOne<User>(e => e.User).WithMany().HasForeignKey(e => e.UserId)
				);
	}

	public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            var entityAudits = OnBeforeSaveChanges();
            var result = base.SaveChanges(acceptAllChangesOnSuccess);
            OnAfterSaveChanges(entityAudits);

            return result;
        }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            var entityAudits = OnBeforeSaveChanges();
            var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            await OnAfterSaveChangesAsync(entityAudits);

            return result;
        }

    private IEnumerable<AuditEntry> OnBeforeSaveChanges()
        {
            ChangeTracker.DetectChanges();
			var auditEntries =
				(from entry in ChangeTracker.Entries()
					where entry.ShouldBeAudited() select new AuditEntry(entry, _auditUserProvider))
				.ToList();

            BeginTrackingAuditEntries(auditEntries.Where(_ => !_.HasTemporaryProperties));

            // keep a list of entries where the value of some properties are unknown at this step
            return auditEntries.Where(_ => _.HasTemporaryProperties);
        }

    private void OnAfterSaveChanges(IEnumerable<AuditEntry> auditEntries)
        {
	        BeginTrackingAuditEntries(auditEntries);

            base.SaveChanges();
        }

    private async Task OnAfterSaveChangesAsync(IEnumerable<AuditEntry> auditEntries)
        {
	        var enumerable = auditEntries.ToList();
	        if (!enumerable.Any())
                return;

            await BeginTrackingAuditEntriesAsync(enumerable);

            await base.SaveChangesAsync();
        }

    private void BeginTrackingAuditEntries(IEnumerable<AuditEntry> auditEntries)
        {
            foreach (var auditEntry in auditEntries)
            {
                auditEntry.Update();
                var auditMetaDataEntity = auditEntry.ToAuditMetaDataEntity();
                var existedAuditMetaDataEntity = AuditMetaData.FirstOrDefault(x => x.HashPrimaryKey == auditMetaDataEntity.HashPrimaryKey && x.SchemaTable == auditMetaDataEntity.SchemaTable);
                Add(existedAuditMetaDataEntity == default
	                ? auditEntry.ToAuditEntity(auditMetaDataEntity)
	                : auditEntry.ToAuditEntity(existedAuditMetaDataEntity));
            }
        }

    private async Task BeginTrackingAuditEntriesAsync(IEnumerable<AuditEntry> auditEntries)
        {
            foreach (var auditEntry in auditEntries)
            {
                auditEntry.Update();
                var auditMetaDataEntity = auditEntry.ToAuditMetaDataEntity();
                var existedAuditMetaDataEntity = await AuditMetaData.FirstOrDefaultAsync(x => x.HashPrimaryKey == auditMetaDataEntity.HashPrimaryKey && x.SchemaTable == auditMetaDataEntity.SchemaTable);
                if (existedAuditMetaDataEntity == default)
                {
                    await AddAsync(auditEntry.ToAuditEntity(auditMetaDataEntity));
                }
                else
                {
                    await AddAsync(auditEntry.ToAuditEntity(existedAuditMetaDataEntity));
                }
            }
        }

}

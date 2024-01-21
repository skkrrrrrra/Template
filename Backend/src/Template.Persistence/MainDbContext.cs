using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Template.Persistence
{
    public class MainDbContext : DbContext
    {
        private readonly IAuditUserProvider? _auditUserProvider;

        public MainDbContext(DbContextOptions options, IAuditUserProvider auditUserProvider) : base(options)
        {
            _auditUserProvider = auditUserProvider;
        }

        public MainDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
    }
}

using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Template.Persistence.Context
{
    public class MainDbContext : IdentityDbContext
    {
        private readonly IAuditUserProvider? _auditUserProvider;

        public MainDbContext(DbContextOptions options, IAuditUserProvider auditUserProvider) : base(options)
        {
            _auditUserProvider = auditUserProvider;
        }

        public MainDbContext(DbContextOptions options) : base(options)
        {
            
        }

        public MainDbContext() { }

        public DbSet<User> Users { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
    }
}

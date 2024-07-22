using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Nelbriz_DataAccess.Data
{
    public class ApplicationDbContext :IdentityDbContext
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor contextAccessor) : base(options)
        {
            _contextAccessor = contextAccessor;
        }


        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Category> Categories { get; set; } 
        public DbSet<Product> Products { get; set; }    
        public DbSet<ProductPrice> ProductPrices { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }    
        public DbSet <OrderDetail> OrderDetails { get; set;}
        public DbSet<AuditLog> AuditLogs { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var modifiedEntities = ChangeTracker.Entries().Where(e => e.State == EntityState.Added ||
                                                                  e.State == EntityState.Modified ||
                                                                  e.State == EntityState.Deleted).ToList();

            foreach (var modifiedEntity in modifiedEntities)
            {
                var auditLog = new AuditLog
                {
                    EntityName = modifiedEntity.Entity.GetType().Name,
                    UserEmail = _contextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name),
                    Action = modifiedEntity.State.ToString(),
                    TimeStamp = DateTime.UtcNow,
                    Changes = GetChanges(modifiedEntity),
                };

                AuditLogs.Add(auditLog);

            }
            return base.SaveChangesAsync(cancellationToken);
        }



        private string GetChanges(EntityEntry modifiedEntity)
        {
            var changes = new StringBuilder();

            foreach (var property in modifiedEntity.OriginalValues.Properties)
            {
                var originalValue = modifiedEntity.OriginalValues[property];
                var currentValue = modifiedEntity.CurrentValues[property];

                if (!Equals(currentValue, originalValue))
                {
                    changes.AppendLine($"{property.Name} : from '{originalValue}' to '{currentValue}'");
                }
            }

            return changes.ToString();
        }



    }
}

using AntonLjunggrenCms.Data.Entites;
using AntonLjunggrenCms.Data.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntonLjunggrenCms.Data.Persistance
{
    public sealed class EfContext : DbContext
    {
        public EfContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<PhotographEntity> Photographs { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.HasDefaultContainer("website-content");

            modelBuilder.Entity<PhotographEntity>()
                .ToContainer("photograph");

            modelBuilder.Entity<PhotographEntity>()
                .HasNoDiscriminator();

            modelBuilder.Entity<PhotographEntity>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<PhotographEntity>()
                .HasPartitionKey(p => p.Id);

            modelBuilder.Entity<PhotographEntity>()
                .Property<bool>("IsActive");

            modelBuilder.Entity<PhotographEntity>()
                .Property<DateTime>("CreatedDate");

            modelBuilder.Entity<PhotographEntity>()
                .Property<DateTime>("ModifiedDate");

            modelBuilder.Entity<PhotographEntity>()
                .HasQueryFilter(p => EF.Property<bool>(p, "IsActive") == true);

            modelBuilder.Entity<PhotographEntity>()
                .Property(p => p.DateTaken)
                .HasConversion<DateOnlyConverter>();

            modelBuilder.Entity<PhotographEntity>()
                .OwnsOne(p => p.Location,
                    navBuilder =>
                    {
                        navBuilder.Property(l => l.Country);
                        navBuilder.Property(l => l.Province);
                        navBuilder.Property(l => l.City);
                    });

            modelBuilder.Entity<PhotographEntity>()
                .OwnsOne(p => p.HdImageData,
                    navBuilder =>
                    {
                        navBuilder.Property(p => p.ImageFilePath);
                        navBuilder.Property(p => p.FileContentType);
                    });

            modelBuilder.Entity<PhotographEntity>()
                .OwnsOne(p => p.SdImageData,
                    navBuilder =>
                    {
                        navBuilder.Property(p => p.ImageFilePath);
                        navBuilder.Property(p => p.FileContentType);
                    });

            base.OnModelCreating(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries().Where(e => e.Entity is PhotographEntity))
            {
                var act = entry.CurrentValues["IsActive"];
                var crea = entry.CurrentValues["CreatedDate"];
                var mod = entry.CurrentValues["ModifiedDate"];

                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.CurrentValues["IsActive"] = true;
                        entry.CurrentValues["CreatedDate"] = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        entry.CurrentValues["ModifiedDate"] = DateTime.UtcNow;
                        break;
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.CurrentValues["IsActive"] = false;
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}

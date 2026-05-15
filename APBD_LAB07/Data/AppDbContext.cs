using APBD_LAB07.Models;
using Microsoft.EntityFrameworkCore;

namespace APBD_LAB07.Data;

public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<PC> PCs { get; set; }
        public DbSet<ComponentType> ComponentTypes { get; set; }
        public DbSet<ComponentManufacturer> ComponentManufacturers { get; set; }
        public DbSet<Component> Components { get; set; }
        public DbSet<PcComponent> PcComponents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PC>(e => {
                e.HasKey(x => x.Id);
                e.Property(x => x.Name).HasMaxLength(50).IsRequired();
                e.Property(x => x.Weight).HasColumnType("float(5)").IsRequired();
                e.Property(x => x.CreatedAt).HasColumnType("datetime").IsRequired();
            });

            modelBuilder.Entity<ComponentType>(e => {
                e.HasKey(x => x.Id);
                e.Property(x => x.Abbreviation).HasMaxLength(30).IsRequired();
                e.Property(x => x.Name).HasMaxLength(150).IsRequired();
            });

            modelBuilder.Entity<ComponentManufacturer>(e => {
                e.HasKey(x => x.Id);
                e.Property(x => x.Abbreviation).HasMaxLength(30).IsRequired();
                e.Property(x => x.FullName).HasMaxLength(300).IsRequired();
                e.Property(x => x.FoundationDate).HasColumnType("date").IsRequired();
            });

            modelBuilder.Entity<Component>(e => {
                e.HasKey(x => x.Code);
                e.Property(x => x.Code).HasColumnType("char(10)").IsRequired();
                e.Property(x => x.Name).HasMaxLength(300).IsRequired();
                e.Property(x => x.Description).HasColumnType("nvarchar(max)").IsRequired();
                e.HasOne(x => x.ComponentManufacturer).WithMany(m => m.Components).HasForeignKey(x => x.ComponentManufacturersId);
                e.HasOne(x => x.ComponentType).WithMany(t => t.Components).HasForeignKey(x => x.ComponentTypesId);
            });

            modelBuilder.Entity<PcComponent>(e => {
                e.HasKey(x => new { x.PCId, x.ComponentCode });
                e.Property(x => x.ComponentCode).HasColumnType("char(10)");
                e.HasOne(x => x.PC).WithMany(p => p.PCComponents).HasForeignKey(x => x.PCId);
                e.HasOne(x => x.Component).WithMany(c => c.PCComponents).HasForeignKey(x => x.ComponentCode);
            });

            modelBuilder.Entity<PC>().HasData(
                new PC { Id = 1, Name = "Gaming Beast X", Weight = 12.5f, Warranty = 36, CreatedAt = DateTime.Parse("2026-05-08T09:00:00"), Stock = 5 },
                new PC { Id = 2, Name = "Office Mini Pro", Weight = 4.2f, Warranty = 24, CreatedAt = DateTime.Parse("2026-04-15T13:30:00"), Stock = 12 },
                new PC { Id = 3, Name = "Home Media Center", Weight = 6.0f, Warranty = 24, CreatedAt = DateTime.Parse("2026-05-01T10:00:00"), Stock = 8 }
            );

            modelBuilder.Entity<ComponentType>().HasData(
                new ComponentType { Id = 1, Abbreviation = "CPU", Name = "Processor" },
                new ComponentType { Id = 2, Abbreviation = "GPU", Name = "Graphics Card" },
                new ComponentType { Id = 3, Abbreviation = "RAM", Name = "Memory" }
            );

            modelBuilder.Entity<ComponentManufacturer>().HasData(
                new ComponentManufacturer { Id = 1, Abbreviation = "AMD", FullName = "Advanced Micro Devices", FoundationDate = DateTime.Parse("1969-05-01") },
                new ComponentManufacturer { Id = 2, Abbreviation = "NV", FullName = "NVIDIA Corporation", FoundationDate = DateTime.Parse("1993-04-05") },
                new ComponentManufacturer { Id = 3, Abbreviation = "COR", FullName = "Corsair Gaming Inc.", FoundationDate = DateTime.Parse("1994-01-01") }
            );

            modelBuilder.Entity<Component>().HasData(
                new Component { Code = "CPU0000001", Name = "Ryzen 7 7800X3D", Description = "8-core gaming processor", ComponentManufacturersId = 1, ComponentTypesId = 1 },
                new Component { Code = "GPU0000001", Name = "RTX 4080 Super", Description = "High-end gaming graphics card", ComponentManufacturersId = 2, ComponentTypesId = 2 },
                new Component { Code = "RAM0000001", Name = "Corsair Vengeance DDR5 16GB", Description = "DDR5 RAM module 16GB", ComponentManufacturersId = 3, ComponentTypesId = 3 }
            );

            modelBuilder.Entity<PcComponent>().HasData(
                new PcComponent { PCId = 1, ComponentCode = "CPU0000001", Amount = 1 },
                new PcComponent { PCId = 1, ComponentCode = "GPU0000001", Amount = 1 },
                new PcComponent { PCId = 1, ComponentCode = "RAM0000001", Amount = 2 }
            );
        }
    }
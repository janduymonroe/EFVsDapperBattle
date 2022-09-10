using EFVsDapperBattle;
using EFVsDapperBattle.Entity;
using Microsoft.EntityFrameworkCore;
using System;

namespace ConsoleApp.Persistence.EF.Context
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Wizzer> Wizzers { get; set; }

        public ApplicationDbContext(DbContextOptions opt) : base(opt)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(ConnectionString.Default);
                optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");

            modelBuilder.Entity<Wizzer>(entity =>
            {
                entity.ToTable("wizzers");
                entity.Property(i => i.Id).UseIdentityColumn(1,1);
                entity.Property(i => i.Nome);
                entity.Property(i => i.Email);
                entity.Property(i => i.Endereco);
                entity.Property(i => i.Telefone);
                entity.Property(i => i.DataNascimento);

            });

            base.OnModelCreating(modelBuilder);
        }
    }
}

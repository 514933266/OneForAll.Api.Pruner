using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Pruner.Domain.Entities;

namespace Pruner.Host
{
    public partial class DDSDbContext : DbContext
    {
        public DDSDbContext(DbContextOptions<DDSDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DsGlobalConfig>(entity =>
            {
                entity.ToTable("ds_global_config");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<DsDeleteConfig>(entity =>
            {
                entity.ToTable("ds_delete_config");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<DsRunningLog>(entity =>
            {
                entity.ToTable("ds_running_log");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<DsDeleteFileConfig>(entity =>
            {
                entity.ToTable("ds_delete_file_config");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<DsMoveFileConfig>(entity =>
            {
                entity.ToTable("ds_move_file_config");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
        }
    }
}

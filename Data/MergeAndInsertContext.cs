using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MergeAndInsert.Data;

public partial class MergeAndInsertContext : DbContext
{
    public MergeAndInsertContext()
    {
    }

    public MergeAndInsertContext(DbContextOptions<MergeAndInsertContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChildTable> ChildTables { get; set; }

    public virtual DbSet<ParentTable> ParentTables { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=MergeAndInsert;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChildTable>(entity =>
        {
            entity.ToTable("ChildTable");

            entity.HasIndex(e => e.ParentTableId, "IX_ChildTable_ParentTableId");

            entity.Property(e => e.SomeMessage).HasMaxLength(4000);
            entity.Property(e => e.StatusDateTimeUtc).HasColumnType("datetime");

            entity.HasOne(d => d.ParentTable).WithMany(p => p.ChildTables)
                .HasForeignKey(d => d.ParentTableId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ParentTable>(entity =>
        {
            entity.ToTable("ParentTable");

            entity.HasIndex(e => e.TraceId, "IX_ParentTable_TraceId");

            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.SomeDateTimeUtc).HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

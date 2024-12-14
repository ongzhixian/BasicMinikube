using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using VirtualFileWebApi.DbModels;

namespace VirtualFileWebApi.DbContexts;

public partial class EimaContext : DbContext
{
    public EimaContext(DbContextOptions<EimaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<VirtualFile> VirtualFiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<VirtualFile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__virtual___3213E83F1966A203");

            entity.ToTable("virtual_file");

            entity.HasIndex(e => e.VirtualPath, "IX_vfile").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FileContent).HasColumnName("file_content");
            entity.Property(e => e.FileSize).HasColumnName("file_size");
            entity.Property(e => e.MimeType)
                .HasMaxLength(255)
                .HasColumnName("mime_type");
            entity.Property(e => e.ModifiedDatetime)
                .HasDefaultValueSql("((sysdatetimeoffset() AT TIME ZONE 'UTC'))")
                .HasColumnName("modified_datetime");
            entity.Property(e => e.VirtualPath)
                .HasMaxLength(255)
                .HasColumnName("virtual_path");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

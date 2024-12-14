using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ObjectStoreWebApi.DbModels;

namespace ObjectStoreWebApi.DbContexts;

public partial class ObjectStorageContext : DbContext
{
    public ObjectStorageContext(DbContextOptions<ObjectStorageContext> options)
        : base(options)
    {
    }

    public virtual DbSet<StorageBucket> StorageBuckets { get; set; }

    public virtual DbSet<StorageObject> StorageObjects { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StorageBucket>(entity =>
        {
            entity.ToTable("storage_bucket");

            entity.HasIndex(e => e.Name, "IX_storage_bucket_name").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateDatetime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("create_datetime");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<StorageObject>(entity =>
        {
            entity.ToTable("storage_object");

            entity.HasIndex(e => e.BucketId, "IX_storage_object_bucket_id").IsUnique();

            entity.HasIndex(e => e.Key, "IX_storage_object_key").IsUnique();

            entity.HasIndex(e => new { e.BucketId, e.Key }, "bucket_object_uidx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BucketId).HasColumnName("bucket_id");
            entity.Property(e => e.Etag).HasColumnName("etag");
            entity.Property(e => e.Key).HasColumnName("key");
            entity.Property(e => e.ModifiedDatetime).HasColumnName("modified_datetime");
            entity.Property(e => e.Size).HasColumnName("size");
            entity.Property(e => e.StorageClass).HasColumnName("storage_class");
            entity.Property(e => e.StoragePath).HasColumnName("storage_path");

            entity.HasOne(d => d.Bucket).WithOne(p => p.StorageObject).HasForeignKey<StorageObject>(d => d.BucketId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

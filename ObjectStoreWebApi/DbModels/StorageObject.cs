using System;
using System.Collections.Generic;

namespace ObjectStoreWebApi.DbModels;

public partial class StorageObject
{
    public int Id { get; set; }

    public string Key { get; set; } = null!;

    public int? BucketId { get; set; }

    public string? Etag { get; set; }

    public int? Size { get; set; }

    public string? ModifiedDatetime { get; set; }

    public string? StorageClass { get; set; }

    public string? StoragePath { get; set; }

    public virtual StorageBucket? Bucket { get; set; }
}

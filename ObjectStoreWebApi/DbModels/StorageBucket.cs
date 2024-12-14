﻿using System;
using System.Collections.Generic;

namespace ObjectStoreWebApi.DbModels;

public partial class StorageBucket
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string CreateDatetime { get; set; } = null!;

    public virtual StorageObject? StorageObject { get; set; }
}
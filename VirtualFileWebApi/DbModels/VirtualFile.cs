using System;
using System.Collections.Generic;

namespace VirtualFileWebApi.DbModels;

public partial class VirtualFile
{
    public int Id { get; set; }

    public string VirtualPath { get; set; } = null!;

    public string MimeType { get; set; } = null!;

    public long FileSize { get; set; }

    public byte[]? FileContent { get; set; }

    public DateTimeOffset ModifiedDatetime { get; set; }
}

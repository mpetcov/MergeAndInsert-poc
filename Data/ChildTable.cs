using System;
using System.Collections.Generic;

namespace MergeAndInsert.Data;

public partial class ChildTable
{
    public int Id { get; set; }

    public int ParentTableId { get; set; }

    public string? SomeMessage { get; set; }

    public DateTime StatusDateTimeUtc { get; set; }

    public virtual ParentTable ParentTable { get; set; } = null!;
}

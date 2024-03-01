using System;
using System.Collections.Generic;

namespace MergeAndInsert.Data;

public partial class ParentTable
{
    public int Id { get; set; }

    public Guid TraceId { get; set; }

    public string Description { get; set; } = null!;

    public DateTime SomeDateTimeUtc { get; set; }

    public virtual ICollection<ChildTable> ChildTables { get; set; } = new List<ChildTable>();
}

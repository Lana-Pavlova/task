using System;
using System.Collections.Generic;

namespace task;

public partial class Materialtype
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public double DefectedPercent { get; set; }

    public virtual ICollection<Material> Materials { get; set; } = new List<Material>();
}

using System;
using System.Collections.Generic;

namespace LibrAI.Data.Entities;

public partial class Publisher
{
    public int id { get; set; }

    public string name { get; set; } = null!;

    public string url { get; set; } = null!;

    public string image_url { get; set; } = null!;
}

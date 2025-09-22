using System;
using System.Collections.Generic;

namespace LibrAI.Data.Entities;

public partial class Book
{
    public int id { get; set; }

    public string? title { get; set; }

    public string? author { get; set; }

    public string? publisher { get; set; }

    public string? isbn { get; set; }

    public int? page_count { get; set; }

    public string? language { get; set; }

    public string? publish_date { get; set; }

    public string? price { get; set; }

    public string? description { get; set; }

    public string? image_url { get; set; }
}

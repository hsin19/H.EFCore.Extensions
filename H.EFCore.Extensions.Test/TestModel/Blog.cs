﻿namespace H.EFCore.Extensions.Test.TestModel;

public class Blog
{
    public int BlogId { get; set; }
    public int UserId { get; set; }
    public string? Name { get; set; }
    public string? Url { get; set; }
}
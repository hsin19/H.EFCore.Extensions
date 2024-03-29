﻿namespace H.EFCore.Extensions.Test.TestModel;

public class Comment
{
    public int PostId { get; set; }
    public int Index { get; set; }
    public int UserId { get; set; }
    public string Content { get; set; } = null!;
    public DateTime Time { get; set; }

    public User User { get; set; } = null!;
}
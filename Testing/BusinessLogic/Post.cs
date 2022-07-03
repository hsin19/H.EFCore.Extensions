namespace IQueryableInvokeReplacer.Testing.BusinessLogic;

public class Post
{
    public int PostId { get; set; }
    public int BlogId { get; set; }
    public string? Title { get; set; } 
    public string? Content { get; set; }
    public DateTime UpdateTime { get; set; }
}
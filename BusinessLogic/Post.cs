namespace IQueryableInvokeReplacer.Testing.BusinessLogic;

public class Post
{
    public int PostId { get; set; }
    public int BlogId { get; set; }
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public DateTime UpdateTime { get; set; }
}
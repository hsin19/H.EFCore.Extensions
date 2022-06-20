namespace IQueryableInvokeReplacer.Testing.BusinessLogic;

public class Comment
{
    public int CommentId { get; set; }
    public int PostId { get; set; }
    public int UserId { get; set; }
    public string Content { get; set; } = null!;
    public DateTime Time { get; set; }

    public User User { get; set; } = null!;
}
namespace IQueryableInvokeReplacer.Testing.BusinessLogic;

public class Blog
{
    public int BlogId { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = null!;
    public string Url { get; set; } = null!;
}
namespace H.EFCore.Extensions.Test.TestModel;

public class BloggingRepository : IBloggingRepository
{
    private readonly BloggingContext _context;

    public BloggingRepository(BloggingContext context)
    {
        _context = context;
    }

    public Blog? GetBlogByName(string name)
    {
        return _context.Blogs.FirstOrDefault(b => b.Name == name);
    }

    public IEnumerable<Blog> GetAllBlogs()
    {
        return _context.Blogs;
    }

    public void AddBlog(Blog blog)
    {
        _context.Add(blog);
    }

    public void SaveChanges()
    {
        _context.SaveChanges();
    }
}
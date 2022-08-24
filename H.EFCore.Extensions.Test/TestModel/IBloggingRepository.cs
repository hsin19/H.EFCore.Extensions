namespace H.EFCore.Extensions.Test.TestModel;

#region IBloggingRepository
public interface IBloggingRepository
{
    Blog? GetBlogByName(string name);

    IEnumerable<Blog> GetAllBlogs();

    void AddBlog(Blog blog);

    void SaveChanges();
}
#endregion
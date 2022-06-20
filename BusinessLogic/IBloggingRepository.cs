using System.Collections.Generic;

namespace IQueryableInvokeReplacer.Testing.BusinessLogic;

#region IBloggingRepository
public interface IBloggingRepository
{
    Blog? GetBlogByName(string name);

    IEnumerable<Blog> GetAllBlogs();

    void AddBlog(Blog blog);

    void SaveChanges();
}
#endregion
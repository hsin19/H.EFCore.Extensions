using H.EFCore.Extensions.Tools;

namespace H.EFCore.Extensions.Test
{
    public class Development_Test : BloggingTestBase
    {
        [Fact]
        public void Type_t()
        {
            var ret1 = typeof(Comment).GetComplexProperty("PostId");
            var a = new { c = new Comment(), p = new Post() };
            var ret2 = a.GetType().GetComplexProperty("PostId");
            var ret3 = a.GetType().GetComplexProperty("BlogId");
            var b = new { u = new User(), cp = new { c = new Comment(), p = new Post() } };
            var ret4 = b.GetType().GetComplexProperty("PostId");
            var ret5 = b.GetType().GetComplexProperty("User.UserId");
            var ret6 = b.GetType().GetComplexProperty("c.User.UserId");
            var ret7 = b.GetType().GetComplexProperty("c.UserId");
            var ret8 = b.GetType().GetComplexProperty("User.Name");
            var ret9 = b.GetType().GetComplexProperty("c.Name");
        }

        [Fact]
        public void Order_t()
        {
            using var context = CreateContext();
            var a = context.Set<Post>().OrderBy("Title").ToQueryString();
            var b = context.Set<Comment>().OrderBy("User.UserId").ToQueryString();
            var c = context.Set<Comment>().OrderBy("PostId").OrderBy("User.UserId").ThenBy("PostId").ToQueryString();
            var d = context.Set<Comment>().OrderThenBy("PostId").OrderThenByDescending("User.UserId").OrderThenBy("Index").ToQueryString();
            var f = context.Set<Comment>().OrderBy("PostId,User.UserId Desc, Index ").ToQueryString();
        }
    }
}

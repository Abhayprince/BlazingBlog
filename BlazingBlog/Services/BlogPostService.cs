using Microsoft.EntityFrameworkCore;

namespace BlazingBlog.Services
{
    public class BlogPostService
    {
        private readonly BlogContext _context;

        public BlogPostService(BlogContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BlogPost>> GetPostsAsync() =>
            await _context.BlogPosts
                           .AsNoTracking()
                            .ToListAsync();
    }
}

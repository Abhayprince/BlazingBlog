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

        public async Task<IEnumerable<BlogPost>> GetPostsAsync(bool publishedOnly = false, string? categorySlug = null)
        {
            var query = _context.BlogPosts
                            .Include(bp => bp.Category)
                           .AsNoTracking();

            if(!string.IsNullOrWhiteSpace(categorySlug))
            {
                var categoryId = await _context.Categories
                                    .AsNoTracking()
                                    .Where(c=> c.Slug == categorySlug)
                                    .Select(c=> c.Id)
                                    .FirstOrDefaultAsync();
                if(categoryId > 0) 
                { 
                    query = query.Where(bp=> bp.CategoryId == categoryId);
                }
            }

            if (publishedOnly)
            {
                query = query.Where(bp => bp.IsPublished);
            }

            return await query.ToListAsync();
        }
        
        public async Task<BlogSaveModel?> GetPostAsync(int blogPostId) =>
            await _context.BlogPosts
                        .Include(bp => bp.Category)
                        .AsNoTracking()
                        .Select(BlogSaveModel.Selector)
                        .FirstOrDefaultAsync(bp => bp.Id == blogPostId);

        public async Task<MethodResult> SaveAsync(BlogSaveModel post, int userId)
        {

            if (post.Id == 0)
            {
                var entity = post.ToBlogEntity(userId);
                // Creating a new blog post
                entity.Slug = entity.Slug.Slugify();

                entity.CreatedOn = DateTime.Now;
                if (entity.IsPublished)
                {
                    entity.PublishedOn = DateTime.Now;
                }

                await _context.AddAsync(entity);
            }
            else
            {
                // Updating an existing blog post

                BlogPost? entity = await _context.BlogPosts
                                    .FirstOrDefaultAsync(bp=> bp.Id == post.Id);
                if(entity is not null)
                {
                    var wasBlogPostPublished = entity.IsPublished;

                    entity = post.Merge(entity);

                    entity.ModifiedOn = DateTime.Now;

                    if (entity.IsPublished)
                    {
                        if(wasBlogPostPublished)
                        {
                            // Do nothing
                        }
                        else
                        {
                            // The blog post was not publsihed in the db
                            // but user published it fromthe ui now
                            entity.PublishedOn = DateTime.Now;
                        }
                    }
                    else if(wasBlogPostPublished)
                    {
                        // This blog post was published earlier in the db
                        // but user now un-published it from the ui

                        entity.PublishedOn = null;
                    }
                }
                else
                {
                    return MethodResult.Failure("This blog post does not exist");
                }
            }

            try
            {
                if (await _context.SaveChangesAsync() > 0)
                {
                    return MethodResult.Succes();
                }
                else
                    return MethodResult.Failure("Unknown error occurred while saving the blog post");
            }
            catch (Exception ex)
            {
                return MethodResult.Failure(ex.Message);
                //throw;
            }
        }
    
        public async Task<BlogPost?> GetPostBySlugAsync(string slug) =>
            await _context.BlogPosts
                        .Include(bp=> bp.Category)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(bp => bp.IsPublished && bp.Slug == slug);
    }
}

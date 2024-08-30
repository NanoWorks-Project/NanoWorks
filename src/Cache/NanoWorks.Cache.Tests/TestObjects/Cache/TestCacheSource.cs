// Ignore Spelling: Nano

using Microsoft.EntityFrameworkCore;
using NanoWorks.Cache.Caches;
using NanoWorks.Cache.Tests.TestObjects.Database;

namespace NanoWorks.Cache.Tests.TestObjects.Cache;

public class TestCacheSource(TestDbContext dbContext) : ICacheSource<AuthorSummary>
{
    public AuthorSummary? Get(string key)
    {
        var author = dbContext.Authors
            .Include(a => a.Books)
            .ThenInclude(a => a.Genre)
            .SingleOrDefault(a => a.Id == Guid.Parse(key));

        if (author is null)
        {
            return null;
        }

        var authorSummary = new AuthorSummary(author);
        return authorSummary;
    }

    public async Task<AuthorSummary?> GetAsync(string key, CancellationToken cancellationToken)
    {
        var author = await dbContext.Authors
            .Include(a => a.Books)
            .ThenInclude(a => a.Genre)
            .SingleOrDefaultAsync(a => a.Id == Guid.Parse(key), cancellationToken);

        if (author is null)
        {
            return null;
        }

        var authorSummary = new AuthorSummary(author);
        return authorSummary;
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Campaign.Data;

public class CampaignDbContextFactory : IDesignTimeDbContextFactory<CampaignDbContext>
{
    public CampaignDbContextFactory()
    {
    }

    public CampaignDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<CampaignDbContext>();
        builder.UseSqlServer("****");
        return new CampaignDbContext(builder.Options);
    }
}
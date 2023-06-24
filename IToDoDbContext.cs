//ממשק שירות שמגדיר את פעולות מסד הנתונים שברצונכם לבצע
using Microsoft.EntityFrameworkCore;
using TodoApi;

public interface IToDoDbContext
{
    DbSet<Item> Items { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
using TodoApi;
using System.Collections;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

public class ItemService
{
    private readonly IToDoDbContext _context;

    public ItemService(IToDoDbContext context)
    {
        _context = context;
    }

[HttpGet("items/getAll")]
public async Task<List<Item>> GetAllItemsAsync()
{
    return await _context.Items.ToListAsync();
}

// [HttpPost("items/add/{name}")]
public async Task AddItemAsync(Item item)
{
    _context.Items.Add(item);
    await _context.SaveChangesAsync();
}

// [HttpPut("items/update")]
public async Task UpdateItemAsync(Item item)
{
    _context.Items.Update(item);
    await _context.SaveChangesAsync();
}

[HttpDelete("items/delete/{id}")]
public async Task DeleteItemAsync(int id)
{
    var item = await _context.Items.FindAsync(id);
    if (item != null)
    {
        _context.Items.Remove(item);
        await _context.SaveChangesAsync();
    }
}
}
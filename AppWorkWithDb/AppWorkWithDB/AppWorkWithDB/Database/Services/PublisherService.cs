using AppWorkWithDB.Database.Contexts;
using AppWorkWithDB.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace AppWorkWithDB.Database.Services;

public class PublisherService
{
    private readonly LibraryDbContext _context;

    public PublisherService(LibraryDbContext context) => _context = context;

    public async Task<List<Publisher>> GetAllPublishersAsync()
    {
        return await _context.Publishers
            .Include(p => p.Books)
            .ToListAsync();
    }

    public async Task<Publisher?> GetPublisherByIdAsync(Guid id)
    {
        return await _context.Publishers
            .Include(p => p.Books)
            .FirstOrDefaultAsync(p => p.PublisherId == id);
    }

    public async Task AddPublisherAsync(Publisher publisher)
    {
        publisher.PublisherId = Guid.NewGuid();
        await _context.Publishers.AddAsync(publisher);
        await _context.SaveChangesAsync();
    }

    public async Task UpdatePublisherAsync(Publisher publisher)
    {
        _context.Publishers.Update(publisher);
        await _context.SaveChangesAsync();
    }

    public async Task DeletePublisherAsync(Guid id)
    {
        var publisher = await _context.Publishers.FindAsync(id);
        if (publisher != null)
        {
            _context.Publishers.Remove(publisher);
            await _context.SaveChangesAsync();
        }
    }
}
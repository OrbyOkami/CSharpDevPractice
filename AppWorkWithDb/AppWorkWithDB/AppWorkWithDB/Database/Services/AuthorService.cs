using AppWorkWithDB.Database.Contexts;
using AppWorkWithDB.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace AppWorkWithDB.Database.Services;

public class AuthorService
{
    private readonly LibraryDbContext _context;

    public AuthorService(LibraryDbContext context) => _context = context;

    public async Task<List<Author>> GetAllAuthorsAsync()
    {
        return await _context.Authors
            .Include(a => a.BookAuthors).ThenInclude(ba => ba.Book)
            .ToListAsync();
    }

    public async Task<Author?> GetAuthorByIdAsync(Guid id)
    {
        return await _context.Authors
            .Include(a => a.BookAuthors).ThenInclude(ba => ba.Book)
            .FirstOrDefaultAsync(a => a.AuthorId == id);
    }

    public async Task AddAuthorAsync(Author author)
    {
        author.AuthorId = Guid.NewGuid();
        await _context.Authors.AddAsync(author);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAuthorAsync(Author author)
    {
        _context.Authors.Update(author);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAuthorAsync(Guid id)
    {
        var author = await _context.Authors.FindAsync(id);
        if (author != null)
        {
            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<Author>> GetLivingAuthorsAsync()
    {
        return await _context.Authors
            .Where(a => a.IsAlive)
            .OrderBy(a => a.LastName)
            .ToListAsync();
    }
}
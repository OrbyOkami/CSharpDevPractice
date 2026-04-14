using AppWorkWithDB.Database.Contexts;
using AppWorkWithDB.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace AppWorkWithDB.Database.Services;

public class BookService
{
    private readonly LibraryDbContext _context;

    public BookService(LibraryDbContext context) => _context = context;

    public async Task<List<Book>> GetAllBooksAsync()
    {
        return await _context.Books
            .Include(b => b.Publisher)
            .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
            .ToListAsync();
    }

    public async Task<Book?> GetBookByIdAsync(Guid id)
    {
        return await _context.Books
            .Include(b => b.Publisher)
            .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
            .FirstOrDefaultAsync(b => b.BookId == id);
    }

    public async Task AddBookAsync(Book book)
    {
        book.BookId = Guid.NewGuid();
        await _context.Books.AddAsync(book);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateBookAsync(Book book)
    {
        _context.Books.Update(book);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteBookAsync(Guid id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book != null)
        {
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<Book>> GetBooksByPublisherAsync(string publisherName, DateTime? fromDate = null)
    {
        var query = _context.Books
            .Include(b => b.Publisher)
            .Where(b => b.Publisher != null && b.Publisher.Name == publisherName);

        if (fromDate.HasValue)
            query = query.Where(b => b.PublicationDate > fromDate.Value);

        return await query.OrderBy(b => b.Title).ToListAsync();
    }
}
namespace AppWorkWithDB.Database.Models;

public class Book
{
    public Guid BookId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? ISBN { get; set; }
    public DateTime? PublicationDate { get; set; }
    public decimal Price { get; set; }
    public int? PageCount { get; set; }
    public bool IsAvailable { get; set; }

    public Guid? PublisherId { get; set; }
    public Publisher? Publisher { get; set; }

    public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
}
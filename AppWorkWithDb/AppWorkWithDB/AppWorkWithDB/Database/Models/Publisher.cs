namespace AppWorkWithDB.Database.Models;

public class Publisher
{
    public Guid PublisherId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? City { get; set; }
    public int? FoundationYear { get; set; }

    public ICollection<Book> Books { get; set; } = new List<Book>();
}
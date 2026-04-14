
using System.Data;
using AppWorkWithDB.Database.Contexts;
using AppWorkWithDB.Database.Models;
using AppWorkWithDB.Database.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

class Program
{
    static async Task Main(string[] args)
    {
        // Конфигурация
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        // DI-контейнер
        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddDbContext<LibraryDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<BookService>();
        services.AddScoped<AuthorService>();
        services.AddScoped<PublisherService>();
        services.AddSingleton<AdoNetService>();

        var serviceProvider = services.BuildServiceProvider();

        // Получаем сервисы
        var bookService = serviceProvider.GetRequiredService<BookService>();
        var authorService = serviceProvider.GetRequiredService<AuthorService>();
        var publisherService = serviceProvider.GetRequiredService<PublisherService>();
        var adoService = serviceProvider.GetRequiredService<AdoNetService>();

        // Главное меню
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== HomeLibrary CRUD ===");
            Console.WriteLine("1. Работа с книгами");
            Console.WriteLine("2. Работа с авторами");
            Console.WriteLine("3. Работа с издательствами");
            Console.WriteLine("4. Отчёты (ADO.NET)");
            Console.WriteLine("0. Выход");
            Console.Write("Выбор: ");
            var choice = Console.ReadLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        await BooksMenu(bookService, publisherService, authorService);
                        break;
                    case "2":
                        await AuthorsMenu(authorService);
                        break;
                    case "3":
                        await PublishersMenu(publisherService);
                        break;
                    case "4":
                        await ReportsMenu(adoService);
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            Console.WriteLine("Нажмите любую клавишу...");
            Console.ReadKey();
        }
    }

    // ----------------- Меню книг -----------------
    static async Task BooksMenu(BookService bookService, PublisherService publisherService, AuthorService authorService)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Книги ===");
            Console.WriteLine("1. Показать все");
            Console.WriteLine("2. Добавить");
            Console.WriteLine("3. Редактировать");
            Console.WriteLine("4. Удалить");
            Console.WriteLine("5. Найти по издательству (с фильтром даты)");
            Console.WriteLine("0. Назад");
            Console.Write("Выбор: ");
            var choice = Console.ReadLine();

            if (choice == "0") break;

            switch (choice)
            {
                case "1":
                    var books = await bookService.GetAllBooksAsync();
                    foreach (var b in books)
                    {
                        Console.WriteLine($"[{b.BookId}] {b.Title} - {b.Publisher?.Name} - {b.Price:C}");
                    }
                    break;
                case "2":
                    await AddBookInteractive(bookService, publisherService, authorService);
                    break;
                case "3":
                    await UpdateBookInteractive(bookService, publisherService);
                    break;
                case "4":
                    await DeleteBookInteractive(bookService);
                    break;
                case "5":
                    await FilterBooksByPublisher(bookService);
                    break;
                default:
                    Console.WriteLine("Неверный выбор");
                    break;
            }
            Console.WriteLine("Нажмите любую клавишу...");
            Console.ReadKey();
        }
    }

    static async Task AddBookInteractive(BookService bookService, PublisherService publisherService, AuthorService authorService)
    {
        Console.Write("Название: ");
        var title = Console.ReadLine();
        Console.Write("ISBN (13 символов): ");
        var isbn = Console.ReadLine();
        Console.Write("Дата публикации (гггг-мм-дд): ");
        DateTime? pubDate = DateTime.TryParse(Console.ReadLine(), out var d) ? d : null;
        Console.Write("Цена: ");
        decimal price = decimal.Parse(Console.ReadLine() ?? "0");
        Console.Write("Количество страниц: ");
        int? pages = int.TryParse(Console.ReadLine(), out var p) ? p : null;

        // Выбор издательства
        var publishers = await publisherService.GetAllPublishersAsync();
        Console.WriteLine("Издательства:");
        for (int i = 0; i < publishers.Count; i++)
            Console.WriteLine($"{i + 1}. {publishers[i].Name}");
        Console.Write("Номер издательства (0 - нет): ");
        int pubIndex = int.Parse(Console.ReadLine() ?? "0");
        Guid? publisherId = pubIndex > 0 ? publishers[pubIndex - 1].PublisherId : null;

        var book = new Book
        {
            Title = title ?? "",
            ISBN = isbn,
            PublicationDate = pubDate,
            Price = price,
            PageCount = pages,
            IsAvailable = true,
            PublisherId = publisherId
        };

        await bookService.AddBookAsync(book);
        Console.WriteLine($"Книга добавлена с ID: {book.BookId}");
    }

    static async Task UpdateBookInteractive(BookService bookService, PublisherService publisherService)
    {
        Console.Write("Введите ID книги: ");
        if (!Guid.TryParse(Console.ReadLine(), out var id))
        {
            Console.WriteLine("Неверный формат GUID");
            return;
        }

        var book = await bookService.GetBookByIdAsync(id);
        if (book == null)
        {
            Console.WriteLine("Книга не найдена");
            return;
        }

        Console.Write($"Название ({book.Title}): ");
        var title = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(title)) book.Title = title;

        Console.Write($"ISBN ({book.ISBN}): ");
        var isbn = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(isbn)) book.ISBN = isbn;

        Console.Write($"Цена ({book.Price}): ");
        if (decimal.TryParse(Console.ReadLine(), out var price)) book.Price = price;

        // Выбор издательства
        var publishers = await publisherService.GetAllPublishersAsync();
        Console.WriteLine("Издательства (0 - оставить текущее):");
        for (int i = 0; i < publishers.Count; i++)
            Console.WriteLine($"{i + 1}. {publishers[i].Name}");
        int pubIndex = int.Parse(Console.ReadLine() ?? "0");
        if (pubIndex > 0) book.PublisherId = publishers[pubIndex - 1].PublisherId;

        await bookService.UpdateBookAsync(book);
        Console.WriteLine("Книга обновлена");
    }

    static async Task DeleteBookInteractive(BookService bookService)
    {
        Console.Write("Введите ID книги: ");
        if (Guid.TryParse(Console.ReadLine(), out var id))
            await bookService.DeleteBookAsync(id);
        else
            Console.WriteLine("Неверный GUID");
    }

    static async Task FilterBooksByPublisher(BookService bookService)
    {
        Console.Write("Название издательства: ");
        var pubName = Console.ReadLine();
        Console.Write("Показать книги, опубликованные после (гггг-мм-дд, Enter - все): ");
        DateTime? from = DateTime.TryParse(Console.ReadLine(), out var dt) ? dt : null;

        var books = await bookService.GetBooksByPublisherAsync(pubName ?? "", from);
        foreach (var b in books)
            Console.WriteLine($"{b.Title} - {b.PublicationDate:yyyy-MM-dd} - {b.Price:C}");
    }

    // ----------------- Меню авторов -----------------
    static async Task AuthorsMenu(AuthorService authorService)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Авторы ===");
            Console.WriteLine("1. Показать всех");
            Console.WriteLine("2. Добавить");
            Console.WriteLine("3. Редактировать");
            Console.WriteLine("4. Удалить");
            Console.WriteLine("5. Показать живущих");
            Console.WriteLine("0. Назад");
            Console.Write("Выбор: ");
            var choice = Console.ReadLine();

            if (choice == "0") break;

            switch (choice)
            {
                case "1":
                    var authors = await authorService.GetAllAuthorsAsync();
                    foreach (var a in authors)
                    {
                        Console.WriteLine($"[{a.AuthorId}] {a.FirstName} {a.LastName} ({(a.IsAlive ? "жив" : "нет")})");
                        foreach (var ba in a.BookAuthors)
                            Console.WriteLine($"   - {ba.Book?.Title}");
                    }
                    break;
                case "2":
                    await AddAuthorInteractive(authorService);
                    break;
                case "3":
                    await UpdateAuthorInteractive(authorService);
                    break;
                case "4":
                    await DeleteAuthorInteractive(authorService);
                    break;
                case "5":
                    var living = await authorService.GetLivingAuthorsAsync();
                    foreach (var a in living)
                        Console.WriteLine($"{a.FirstName} {a.LastName}");
                    break;
                default:
                    Console.WriteLine("Неверный выбор");
                    break;
            }
            Console.WriteLine("Нажмите любую клавишу...");
            Console.ReadKey();
        }
    }

    static async Task AddAuthorInteractive(AuthorService authorService)
    {
        Console.Write("Имя: ");
        var firstName = Console.ReadLine();
        Console.Write("Фамилия: ");
        var lastName = Console.ReadLine();
        Console.Write("Дата рождения (гггг-мм-дд): ");
        DateTime? birth = DateTime.TryParse(Console.ReadLine(), out var b) ? b : null;
        Console.Write("Жив? (y/n): ");
        bool isAlive = Console.ReadLine()?.Trim().ToLower() == "y";

        var author = new Author
        {
            FirstName = firstName ?? "",
            LastName = lastName ?? "",
            BirthDate = birth,
            IsAlive = isAlive
        };

        await authorService.AddAuthorAsync(author);
        Console.WriteLine($"Автор добавлен с ID: {author.AuthorId}");
    }

    static async Task UpdateAuthorInteractive(AuthorService authorService)
    {
        Console.Write("Введите ID автора: ");
        if (!Guid.TryParse(Console.ReadLine(), out var id))
        {
            Console.WriteLine("Неверный GUID");
            return;
        }

        var author = await authorService.GetAuthorByIdAsync(id);
        if (author == null)
        {
            Console.WriteLine("Автор не найден");
            return;
        }

        Console.Write($"Имя ({author.FirstName}): ");
        var firstName = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(firstName)) author.FirstName = firstName;

        Console.Write($"Фамилия ({author.LastName}): ");
        var lastName = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(lastName)) author.LastName = lastName;

        Console.Write($"Жив? (y/n) [{(author.IsAlive ? "y" : "n")}]: ");
        var aliveInput = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(aliveInput))
            author.IsAlive = aliveInput.Trim().ToLower() == "y";

        await authorService.UpdateAuthorAsync(author);
        Console.WriteLine("Автор обновлён");
    }

    static async Task DeleteAuthorInteractive(AuthorService authorService)
    {
        Console.Write("Введите ID автора: ");
        if (Guid.TryParse(Console.ReadLine(), out var id))
            await authorService.DeleteAuthorAsync(id);
        else
            Console.WriteLine("Неверный GUID");
    }

    // ----------------- Меню издательств -----------------
    static async Task PublishersMenu(PublisherService publisherService)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Издательства ===");
            Console.WriteLine("1. Показать все");
            Console.WriteLine("2. Добавить");
            Console.WriteLine("3. Редактировать");
            Console.WriteLine("4. Удалить");
            Console.WriteLine("0. Назад");
            Console.Write("Выбор: ");
            var choice = Console.ReadLine();

            if (choice == "0") break;

            switch (choice)
            {
                case "1":
                    var pubs = await publisherService.GetAllPublishersAsync();
                    foreach (var p in pubs)
                    {
                        Console.WriteLine($"[{p.PublisherId}] {p.Name} ({p.City}) - книг: {p.Books.Count}");
                    }
                    break;
                case "2":
                    await AddPublisherInteractive(publisherService);
                    break;
                case "3":
                    await UpdatePublisherInteractive(publisherService);
                    break;
                case "4":
                    await DeletePublisherInteractive(publisherService);
                    break;
                default:
                    Console.WriteLine("Неверный выбор");
                    break;
            }
            Console.WriteLine("Нажмите любую клавишу...");
            Console.ReadKey();
        }
    }

    static async Task AddPublisherInteractive(PublisherService publisherService)
    {
        Console.Write("Название: ");
        var name = Console.ReadLine();
        Console.Write("Город: ");
        var city = Console.ReadLine();
        Console.Write("Год основания: ");
        int? year = int.TryParse(Console.ReadLine(), out var y) ? y : null;

        var pub = new Publisher
        {
            Name = name ?? "",
            City = city,
            FoundationYear = year
        };
        await publisherService.AddPublisherAsync(pub);
        Console.WriteLine($"Издательство добавлено с ID: {pub.PublisherId}");
    }

    static async Task UpdatePublisherInteractive(PublisherService publisherService)
    {
        Console.Write("Введите ID издательства: ");
        if (!Guid.TryParse(Console.ReadLine(), out var id))
        {
            Console.WriteLine("Неверный GUID");
            return;
        }

        var pub = await publisherService.GetPublisherByIdAsync(id);
        if (pub == null)
        {
            Console.WriteLine("Издательство не найдено");
            return;
        }

        Console.Write($"Название ({pub.Name}): ");
        var name = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(name)) pub.Name = name;

        Console.Write($"Город ({pub.City}): ");
        var city = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(city)) pub.City = city;

        Console.Write($"Год основания ({pub.FoundationYear}): ");
        if (int.TryParse(Console.ReadLine(), out var year)) pub.FoundationYear = year;

        await publisherService.UpdatePublisherAsync(pub);
        Console.WriteLine("Издательство обновлено");
    }

    static async Task DeletePublisherInteractive(PublisherService publisherService)
    {
        Console.Write("Введите ID издательства: ");
        if (Guid.TryParse(Console.ReadLine(), out var id))
            await publisherService.DeletePublisherAsync(id);
        else
            Console.WriteLine("Неверный GUID");
    }

    // ----------------- Меню отчётов (ADO.NET) -----------------
    static async Task ReportsMenu(AdoNetService adoService)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Отчёты (ADO.NET) ===");
            Console.WriteLine("1. Издательства с количеством книг > N");
            Console.WriteLine("2. Количество книг у авторов");
            Console.WriteLine("3. Массовое повышение цен");
            Console.WriteLine("0. Назад");
            Console.Write("Выбор: ");
            var choice = Console.ReadLine();

            if (choice == "0") break;

            switch (choice)
            {
                case "1":
                    Console.Write("Минимальное количество книг: ");
                    int min = int.Parse(Console.ReadLine() ?? "5");
                    var dt = adoService.GetPublisherBookCounts(min);
                    foreach (DataRow row in dt.Rows)
                        Console.WriteLine($"{row["Name"]}: {row["BookCount"]}");
                    break;
                case "2":
                    var authorsDt = adoService.GetAuthorsWithBookCount();
                    foreach (DataRow row in authorsDt.Rows)
                        Console.WriteLine($"{row["AuthorName"]}: {row["BookCount"]} книг");
                    break;
                case "3":
                    Console.Write("Порог цены: ");
                    decimal threshold = decimal.Parse(Console.ReadLine() ?? "0");
                    Console.Write("Процент повышения: ");
                    decimal percent = decimal.Parse(Console.ReadLine() ?? "0");
                    int affected = adoService.BulkUpdatePrice(threshold, percent);
                    Console.WriteLine($"Обновлено записей: {affected}");
                    break;
                default:
                    Console.WriteLine("Неверный выбор");
                    break;
            }
            Console.WriteLine("Нажмите любую клавишу...");
            Console.ReadKey();
        }
    }
}
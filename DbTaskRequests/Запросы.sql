-- ================================================
-- ЗАПРОСЫ ДЛЯ БАЗЫ ДАННЫХ HomeLibrary
-- (выборка, изменение, группировка, соединения)
-- ================================================
USE HomeLibrary;
GO

-- ================================================
-- 5.1. Выборка с фильтрацией и сортировкой
-- Найти книги издательства "АСТ", опубликованные после 1950 года
-- ================================================
SELECT Title, PublicationDate, Price
FROM Books
WHERE 
    PublisherID = (SELECT PublisherID FROM Publishers WHERE Name = N'АСТ')
    AND PublicationDate > '1950-01-01'
ORDER BY Title ASC;
GO

-- ================================================
-- 5.2. Изменение данных (UPDATE)
-- Увеличить цену на 10% для книг дороже 500 рублей
-- (транзакция с откатом для демонстрации)
-- ================================================
BEGIN TRANSACTION;

UPDATE Books
SET Price = Price * 1.10
WHERE Price > 500;

-- Проверка результата
SELECT Title, Price FROM Books WHERE Price > 500 ORDER BY Price DESC;

ROLLBACK TRANSACTION; -- Отменяем изменения
-- COMMIT TRANSACTION; -- Раскомментируйте, чтобы сохранить
GO

-- ================================================
-- 5.3. Удаление данных (DELETE)
-- Удалить книгу "Скотный двор"
-- (транзакция с откатом для демонстрации)
-- ================================================
BEGIN TRANSACTION;

DELETE FROM Books
WHERE BookID = (SELECT BookID FROM Books WHERE Title = N'Скотный двор');

-- Проверка
SELECT Title FROM Books WHERE Title = N'Скотный двор'; -- Пусто

ROLLBACK TRANSACTION;
-- COMMIT TRANSACTION;
GO

-- ================================================
-- 5.4. Группировка с HAVING
-- Издательства, у которых больше 5 книг
-- ================================================
SELECT 
    p.Name AS PublisherName,
    COUNT(b.BookID) AS BookCount
FROM Publishers p
LEFT JOIN Books b ON p.PublisherID = b.PublisherID
GROUP BY p.Name
HAVING COUNT(b.BookID) > 5
ORDER BY BookCount DESC;
GO

-- ================================================
-- 5.5. LEFT JOIN – все авторы и их книги
-- ================================================
SELECT 
    a.FirstName + ' ' + a.LastName AS AuthorName,
    ISNULL(b.Title, N'<нет книг>') AS BookTitle
FROM Authors a
LEFT JOIN BookAuthors ba ON a.AuthorID = ba.AuthorID
LEFT JOIN Books b ON ba.BookID = b.BookID
ORDER BY AuthorName, BookTitle;
GO

-- ================================================
-- 5.6. INNER JOIN – книги живых авторов
-- ================================================
SELECT 
    b.Title,
    a.FirstName + ' ' + a.LastName AS AuthorName
FROM Books b
INNER JOIN BookAuthors ba ON b.BookID = ba.BookID
INNER JOIN Authors a ON ba.AuthorID = a.AuthorID
WHERE a.IsAlive = 1
ORDER BY b.Title;
GO
-- ================================================
-- 1. СОЗДАНИЕ БАЗЫ ДАННЫХ И ПЕРЕКЛЮЧЕНИЕ В НЕЁ
-- ================================================
CREATE DATABASE HomeLibrary;
GO

USE HomeLibrary;
GO

-- ================================================
-- 2. СОЗДАНИЕ ТАБЛИЦ
-- ================================================

-- Таблица "Авторы"
CREATE TABLE Authors (
    AuthorID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(), -- GUID
    FirstName NVARCHAR(50) NOT NULL,                       -- Строка
    LastName NVARCHAR(50) NOT NULL,                        -- Строка
    BirthDate DATE,                                        -- Дата
    IsAlive BIT DEFAULT 1                                  -- Булевый тип (1 - жив, 0 - нет)
);

-- Таблица "Издательства"
CREATE TABLE Publishers (
    PublisherID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(), -- GUID
    Name NVARCHAR(100) NOT NULL,                              -- Строка
    City NVARCHAR(50),                                        -- Строка
    FoundationYear INT                                        -- Число
);

-- Таблица "Книги" (без внешнего ключа пока)
CREATE TABLE Books (
    BookID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(), -- GUID
    Title NVARCHAR(200) NOT NULL,                        -- Строка
    ISBN CHAR(13),                                       -- Строка фиксированной длины
    PublicationDate DATE,                                -- Дата
    Price DECIMAL(10, 2),                                -- Десятичное число
    PageCount INT,                                       -- Число
    IsAvailable BIT DEFAULT 1,                           -- Булевый тип
    PublisherID UNIQUEIDENTIFIER NULL                    -- Поле для связи с Publishers
);

-- Таблица "Жанры"
CREATE TABLE Genres (
    GenreID INT IDENTITY(1,1) PRIMARY KEY,   -- Автоинкремент
    Name NVARCHAR(50) NOT NULL UNIQUE        -- Уникальная строка
);

-- Промежуточная таблица для связи "Многие ко многим" (Книги ↔ Авторы)
CREATE TABLE BookAuthors (
    BookID UNIQUEIDENTIFIER NOT NULL,
    AuthorID UNIQUEIDENTIFIER NOT NULL,
    PRIMARY KEY (BookID, AuthorID)           -- Составной первичный ключ
);
GO

-- ================================================
-- 3. ДОБАВЛЕНИЕ СВЯЗЕЙ (ВНЕШНИХ КЛЮЧЕЙ)
-- ================================================

-- Связь 1:N (Издательство → Книги)
ALTER TABLE Books
ADD CONSTRAINT FK_Books_Publishers 
    FOREIGN KEY (PublisherID) 
    REFERENCES Publishers(PublisherID) 
    ON DELETE SET NULL;    -- При удалении издательства в книгах PublisherID станет NULL

-- Связь N:N (Книги ↔ Авторы через BookAuthors)
ALTER TABLE BookAuthors
ADD CONSTRAINT FK_BookAuthors_Books 
    FOREIGN KEY (BookID) 
    REFERENCES Books(BookID) 
    ON DELETE CASCADE;     -- При удалении книги удаляются все её связи с авторами

ALTER TABLE BookAuthors
ADD CONSTRAINT FK_BookAuthors_Authors 
    FOREIGN KEY (AuthorID) 
    REFERENCES Authors(AuthorID) 
    ON DELETE CASCADE;     -- При удалении автора удаляются все его связи с книгами
GO

-- ================================================
-- 4. УНИКАЛЬНЫЙ ИНДЕКС НА ПОЛЕ ISBN
-- ================================================
CREATE UNIQUE INDEX IX_Books_ISBN ON Books(ISBN)
WHERE ISBN IS NOT NULL;   -- Игнорируем NULL, чтобы можно было вставлять книги без ISBN
GO
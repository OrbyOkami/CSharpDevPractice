# Приложение учёта рабочего времени (TimeCRM)

Веб-приложение для учёта рабочего времени сотрудников с использованием ASP.NET Core 10, PostgreSQL и Vue.js. Полностью контейнеризировано с помощью Docker.

## 🚀 Быстрый старт с Docker Compose

### Предварительные требования

- Установленный [Docker](https://docs.docker.com/get-docker/) и Docker Compose.
- Свободные порты `8080` (фронтенд), `5000` (бэкенд API) и `5433` (PostgreSQL, если изменён).  
  *При необходимости можно изменить внешние порты в файле `docker-compose.yml`.*

### Запуск приложения

1. **Склонируйте репозиторий** или убедитесь, что находитесь в корневой папке `CSharpDevPractice`, содержащей:
   - `docker-compose.yml`
   - `Dockerfile.backend`
   - `Dockerfile.frontend`
   - `nginx.conf`
   - Папки `TimeCrm/` и `TimeCrmWebApp/vue-time-tracker/`

2. **Опционально:** создайте файл `.env` для переопределения учётных данных БД:
   ```env
   POSTGRES_USER=postgres
   POSTGRES_PASSWORD=YourStrongPassword
   POSTGRES_DB=TimeTrackingDb
   ```
   По умолчанию используются значения из `docker-compose.yml`.

3. **Запустите контейнеры** в фоновом режиме:
   ```bash
   docker-compose up -d
   ```

4. **Дождитесь запуска всех сервисов** (примерно 10-20 секунд). Можно проверить статус:
   ```bash
   docker-compose ps
   ```

5. **Откройте приложение** в браузере:
   - Веб-интерфейс: [http://localhost:8080](http://localhost:8080)
   - Swagger-документация API: [http://localhost:5000/swagger](http://localhost:5000/swagger)

### Остановка приложения

```bash
docker-compose down
```

Данные БД сохраняются в томе `postgres_data`. Чтобы удалить том вместе с данными:
```bash
docker-compose down -v
```

---

## 🌐 Доступные эндпоинты API

Базовый URL API (внутри контейнера `http://backend:80`, с хоста `http://localhost:5000`).

### Проекты

| Метод | Эндпоинт            | Описание                       |
|-------|---------------------|--------------------------------|
| GET   | `/api/projects`     | Список всех проектов           |
| GET   | `/api/projects/{id}`| Получить проект по ID          |
| POST  | `/api/projects`     | Создать новый проект           |
| PUT   | `/api/projects/{id}`| Обновить проект                |
| DELETE| `/api/projects/{id}`| Удалить проект                 |

### Задачи

| Метод | Эндпоинт               | Описание                        |
|-------|------------------------|---------------------------------|
| GET   | `/api/tasks`           | Список всех задач               |
| GET   | `/api/tasks/active`    | Только активные задачи          |
| GET   | `/api/tasks/{id}`      | Получить задачу по ID           |
| POST  | `/api/tasks`           | Создать задачу                  |
| PUT   | `/api/tasks/{id}`      | Обновить задачу                 |
| DELETE| `/api/tasks/{id}`      | Удалить задачу                  |

### Проводки (списание времени)

| Метод | Эндпоинт                           | Описание                                |
|-------|------------------------------------|-----------------------------------------|
| GET   | `/api/timeentries`                 | Все проводки (поддерживает фильтры)     |
| GET   | `/api/timeentries?date=2026-04-15` | Проводки за конкретный день             |
| GET   | `/api/timeentries?month=4&year=2026` | Проводки за месяц                      |
| GET   | `/api/timeentries/{id}`            | Получить проводку по ID                 |
| POST  | `/api/timeentries`                 | Создать новую проводку                  |
| PUT   | `/api/timeentries/{id}`            | Обновить проводку                       |
| DELETE| `/api/timeentries/{id}`            | Удалить проводку                        |
| GET   | `/api/timeentries/daily-summary?date=2026-04-15` | Сводка часов за день (и цвет стикера) |

### Проверка здоровья (health check)

Контейнеры не имеют специальных health-check эндпоинтов, но API отвечает на стандартные запросы. PostgreSQL проверяется встроенным механизмом Docker Compose.

---

## 🛠️ Переменные окружения

Можно задать в файле `.env` или прямо в `docker-compose.yml`:

| Переменная         | Назначение                              | Значение по умолчанию  |
|--------------------|-----------------------------------------|------------------------|
| `POSTGRES_USER`    | Имя пользователя PostgreSQL             | `postgres`             |
| `POSTGRES_PASSWORD`| Пароль пользователя БД                  | `YourStrongPassword`   |
| `POSTGRES_DB`      | Имя базы данных                         | `TimeTrackingDb`       |

Строка подключения формируется автоматически и передаётся в бэкенд.

---

## 📁 Структура Docker-образов

- **backend** – ASP.NET Core 10 на порту 5000 (внутри 80).  
  Автоматически применяет миграции Entity Framework при старте.
- **frontend** – Vue.js, собирается Vite и раздаётся Nginx на порту 8080.  
  Проксирует запросы к `/api` на бэкенд.
- **postgres** – PostgreSQL 16, данные сохраняются в томе `postgres_data`.

---

## 🧪 Тестирование API

Вы можете использовать Swagger UI по адресу [http://localhost:5000/swagger](http://localhost:5000/swagger) для интерактивной отправки запросов.

Пример запроса через `curl`:
```bash
curl -X POST http://localhost:5000/api/projects \
  -H "Content-Type: application/json" \
  -d '{"name": "Новый проект", "code": "PROJ1", "isActive": true}'
```

---

## ❗ Возможные проблемы

1. **Порт 5432 занят** – измените маппинг порта PostgreSQL в `docker-compose.yml` с `5432:5432` на `5433:5432`.
2. **Не создаются таблицы в БД** – убедитесь, что в `Program.cs` бэкенда присутствует вызов `dbContext.Database.Migrate()`.
3. **Стили не применяются на фронте** – возможно, Tailwind CSS не был корректно установлен. Пересоберите фронтенд с флагом `--no-cache`.

При возникновении ошибок просмотрите логи контейнера:
```bash
docker-compose logs backend
docker-compose logs frontend
docker-compose logs postgres
```

---

## 📄 Лицензия

Проект разработан в учебных целях. Используйте свободно.
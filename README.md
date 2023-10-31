# SimbirGO

## Инструкция по запуску

1. Перейти по пути `путь_до_проекта\SimbirGO`.
2. Открыть файл `.sln` через MS Visual Studio/Jetbrains Rider.
3. Нажать кнопку Run (зеленая стрелка).

Или

1. Перейти по пути `путь_до_проекта\SimbirGO\SimbirGOSwagger`.
2. Открыть консоль в этом каталоге.
3. Выполнить команду `dotnet run`.

### URL Swagger UI:
[http://localhost:5050/swagger/index.html](http://localhost:5050/swagger/index.html)

Чтобы взаимодействовать напрямую с API, обращайтесь по URL:

[http://localhost:5050/api/то_что_вам_нужно](http://localhost:5050/api/то_что_вам_нужно)

## Возможные проблемы

В файле по пути `путь_до_проекта\SimbirGO\SimbirGOSwagger\appsettings.json` расположены настройки подключения к базе данных. Поскольку у разных пользователей могут быть разные пароли, вам придется немного изменить настройки под себя, как минимум поменять пароль для доступа к локальному серверу PostgreSQL.

Строчка для изменения:

```json
"DefaultConnection": "Server=localhost;Port=5432;Database=simbir;User Id=postgres;Password=your_password"

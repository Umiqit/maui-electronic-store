# Электронный магазин для предприятия (.NET MAUI + PostgreSQL)

Лабораторные работы по разработке оконного приложения электронного магазина на .NET MAUI.

## Состав

| Папка | Содержимое |
|---|---|
| `Lab1/` | ЛР1 — создание проекта и разработка интерфейса пользователя (меню, панель инструментов) |
| `Lab2/` | ЛР2–ЛР7 — бизнес-логика (ICommand), EDM-модель данных (EF Core + Npgsql), привязка данных к CollectionView, методы манипулирования данными (CRUD), валидация (CommunityToolkit.Maui), поиск данных |
| `sql/` | `shopdb_postgres.sql` — скрипт создания базы данных PostgreSQL `shopdb` (таблицы, связи, индексы, тестовые данные, представление `sales_report`) |

## База данных (ЛР3)

1. Установите PostgreSQL и pgAdmin.
2. Откройте pgAdmin → Query Tool, выполните скрипт `sql/shopdb_postgres.sql` — будет создана база `shopdb` с тестовыми данными.
3. Проверьте строку подключения в `Lab2/MauiApp2 Lab2/Data/ShopContext.cs` (по умолчанию `Host=localhost;Port=5432;Database=shopdb;Username=postgres;Password=123456789`) — при необходимости замените пароль на свой.

## Сборка и запуск

1. Откройте `Lab2/MauiApp2 Lab2.slnx` в Visual Studio (требуется .NET 10 SDK с workload MAUI).
2. Дождитесь восстановления NuGet-пакетов (`Npgsql.EntityFrameworkCore.PostgreSQL`, `Microsoft.EntityFrameworkCore.Tools`, `CommunityToolkit.Maui`).
3. Выберите цель запуска **Windows Machine** и нажмите F5.
4. Убедитесь, что PostgreSQL запущен — иначе приложение покажет диалог с ошибкой подключения.

## NuGet-пакеты (Лабораторная работа №3)

```powershell
# Package Manager Console (для Scaffold-DbContext)
Scaffold-DbContext "Host=localhost;Port=5432;Database=shopdb;Username=postgres;Password=123456789" Npgsql.EntityFrameworkCore.PostgreSQL
```

# Электронный магазин для предприятия (.NET MAUI + PostgreSQL)

Законченное оконное приложение электронного магазина, разработанное в ходе лабораторных работ №1–7 по .NET MAUI.

## Состав репозитория

- `ElectronicStore.slnx` — решение Visual Studio
- `ElectronicStore/` — проект приложения
- `sql/shopdb_postgres.sql` — скрипт создания базы данных PostgreSQL `shopdb` (таблицы, связи, индексы, тестовые данные, представление `sales_report`)

## Функциональность по лабораторным работам

| ЛР | Что реализовано | Где в коде |
|---|---|---|
| ЛР1 | Интерфейс пользователя: меню «Действие»/«Отчёт», панель инструментов | `AppShell.xaml`, `HomePage.xaml` |
| ЛР2 | Бизнес-логика: модель команд `ICommand` | команды в `HomePage.xaml.cs` |
| ЛР3 | EDM-модель данных: EF Core + Npgsql, контекст и модели БД | `Data/ShopContext.cs`, `Models/` |
| ЛР4 | Привязка данных: `ObservableCollection` + `CollectionView` | `HomePage.xaml`, `LoadProducts()` |
| ЛР5 | Манипулирование данными: Edit/Save/Delete/Undo, страницы добавления товара и покупателя | `HomePage.xaml.cs`, `AddProductPage`, `AddCustomerPage` |
| ЛР6 | Валидация данных: `TextValidationBehavior`, `NumericValidationBehavior` (CommunityToolkit.Maui) | поведения в XAML-страницах |
| ЛР7 | Поиск данных: по названию товара и категории через `EF.Functions.Like` | `SearchButton_Clicked` в `HomePage.xaml.cs` |

## База данных

1. Установите PostgreSQL и pgAdmin.
2. Откройте pgAdmin → Query Tool, выполните скрипт `sql/shopdb_postgres.sql` — будет создана база `shopdb` с тестовыми данными.
3. Строка подключения в `ElectronicStore/Data/ShopContext.cs`: `Host=localhost;Port=5432;Database=shopdb;Username=postgres;Password=umiqit1234` — при необходимости замените пароль на свой.

## Оформление и мультимедиа

- Дизайн в стиле Fluent (как у сайта Microsoft): фирменный синий `#0F6CBD`, белые карточки со скруглениями, плоские поля ввода.
- Звук успеха (`Resources/Raw/success.wav`, воспроизведение через Plugin.Maui.Audio) при сохранении, удалении и добавлении записи.
- Анимации: плавное появление списка и панели поиска, всплывающий статус-баннер с результатом действия, «нажатие» кнопок.

## Сборка и запуск

1. Откройте `ElectronicStore.slnx` в Visual Studio (требуется .NET 10 SDK с workload MAUI).
2. Дождитесь восстановления NuGet-пакетов (`Npgsql.EntityFrameworkCore.PostgreSQL 10.0.3`, `Microsoft.EntityFrameworkCore.Tools 10.0.0`, `CommunityToolkit.Maui 13.0.0`).
3. Выберите цель запуска **Windows Machine** и нажмите F5.
4. Убедитесь, что PostgreSQL запущен — иначе приложение покажет диалог с ошибкой подключения.

> **Важно:** если приложение падает при запуске под отладчиком (F5), но работает без него (Ctrl+F5) — отключите инструменты диагностики XAML: Средства → Параметры → Отладка → Диагностика XAML → снимите флажки. Это известный конфликт дизайнера WinUI с .NET MAUI, к самому приложению отношения не имеет.

## NuGet-пакеты (ЛР3)

```powershell
# Package Manager Console (для Scaffold-DbContext)
Scaffold-DbContext "Host=localhost;Port=5432;Database=shopdb;Username=postgres;Password=123456789" Npgsql.EntityFrameworkCore.PostgreSQL
```

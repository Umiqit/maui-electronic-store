using System.Collections.ObjectModel;
using System.Windows.Input;
using ElectronicStore.Data;
using ElectronicStore.Models;
using ElectronicStore.Services;
using Microsoft.EntityFrameworkCore;

namespace ElectronicStore;

public partial class HomePage : ContentPage
{
    // Контекст данных (ЛР3) и коллекция для работы с данными (ЛР4)
    private readonly ShopContext shopContext;
    public ObservableCollection<Product> Products { get; set; }

    // Команды интерфейса ICommand (ЛР2, ЛР5)
    public ICommand CancelCommand { get; private set; }
    public ICommand CreateCommand { get; private set; }
    public ICommand EditCommand { get; private set; }
    public ICommand SaveCommand { get; private set; }
    public ICommand FindCommand { get; private set; }
    public ICommand DeleteCommand { get; private set; }
    public ICommand ReportCommand { get; private set; }
    public ICommand AddCustomerCommand { get; private set; }

    // Счётчик версий для статус-баннера (чтобы анимации не перекрывались)
    private int statusVersion;

    public HomePage()
    {
        CrashLog.Write("HomePage: конструктор, начало");
        InitializeComponent();

        shopContext = new ShopContext();
        Products = new ObservableCollection<Product>();
        CrashLog.Write("HomePage: конструктор, контекст создан");

        // Отмена: возвращает список к данным из базы (ЛР5)
        CancelCommand = new Command(execute: () => LoadProducts());

        // Создать: переход на страницу добавления товара (ЛР5)
        CreateCommand = new Command(execute: async () =>
            await Navigation.PushAsync(new AddProductPage()));

        // Редактировать: помечает выбранный товар как изменённый (ЛР5)
        EditCommand = new Command(execute: EditHandler);

        // Сохранить: фиксирует изменения в базе данных (ЛР5)
        SaveCommand = new Command(execute: SaveHandler);

        // Найти: фокус на панели поиска (ЛР7)
        FindCommand = new Command(execute: () => SearchName.Focus());

        // Удалить: помечает выбранный товар к удалению (ЛР5)
        DeleteCommand = new Command(execute: DeleteHandler);

        ReportCommand = new Command(execute: async () =>
            await DisplayAlertAsync("Отчёт", "Формирование отчёта", "Ок"));

        AddCustomerCommand = new Command(execute: async () =>
            await Navigation.PushAsync(new AddCustomerPage()));

        this.BindingContext = this;
    }

    // Загрузка данных при появлении страницы (ЛР4) + анимация появления
    protected override void OnAppearing()
    {
        base.OnAppearing();
        CrashLog.Write("HomePage: OnAppearing");

        collectionView.Opacity = 0;
        collectionView.TranslationY = 24;
        SearchPanel.Opacity = 0;
        SearchPanel.TranslationY = 16;

        CrashLog.Write("HomePage: загрузка товаров из БД");
        LoadProducts();
        CrashLog.Write("HomePage: загрузка завершена");

        _ = Task.WhenAll(
            collectionView.FadeToAsync(1, 400, Easing.CubicOut),
            collectionView.TranslateToAsync(0, 0, 400, Easing.CubicOut),
            SearchPanel.FadeToAsync(1, 400, Easing.CubicOut),
            SearchPanel.TranslateToAsync(0, 0, 400, Easing.CubicOut));
    }

    private void LoadProducts()
    {
        try
        {
            var products = shopContext.Products
                .Include(p => p.Category)
                .ToList();
            Products = new ObservableCollection<Product>(products);
            this.collectionView.ItemsSource = Products;
        }
        catch (Exception ex)
        {
            // Если PostgreSQL не запущен или база shopdb не создана —
            // показываем сообщение вместо падения приложения.
            MainThread.BeginInvokeOnMainThread(async () =>
                await DisplayAlertAsync("Ошибка подключения к базе данных",
                    $"Проверьте, что PostgreSQL запущен, база shopdb создана скриптом shopdb_postgres.sql, а строка подключения в ShopContext.cs верна.\n\n{ex.Message}",
                    "Ок"));
        }
    }

    // Показ баннера статуса с анимацией (всплывает и растворяется)
    private async void ShowStatus(string text, Color color)
    {
        var version = ++statusVersion;
        StatusLabel.Text = text;
        StatusLabel.TextColor = color;
        StatusLabel.Opacity = 0;
        StatusLabel.TranslationY = 8;

        await Task.WhenAll(
            StatusLabel.FadeToAsync(1, 200, Easing.CubicOut),
            StatusLabel.TranslateToAsync(0, 0, 200, Easing.CubicOut));

        await Task.Delay(1600);
        if (version != statusVersion)
        {
            return; // Успело появиться новое сообщение — не гасим его
        }

        await Task.WhenAll(
            StatusLabel.FadeToAsync(0, 300, Easing.CubicIn),
            StatusLabel.TranslateToAsync(0, -6, 300, Easing.CubicIn));
    }

    private void EditHandler(object sender)
    {
        if (this.collectionView.SelectedItem is Product product)
        {
            this.shopContext.Products.Update(product);
        }
    }

    private async void SaveHandler(object sender)
    {
        try
        {
            this.shopContext.SaveChanges();
            SoundService.PlaySuccess();
            ShowStatus("Изменения сохранены", Color.FromArgb("#107C10"));
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Ошибка", ex.Message, "Ок");
        }
    }

    private void DeleteHandler(object sender)
    {
        if (this.collectionView.SelectedItem is Product product)
        {
            this.shopContext.Products.Remove(product);
            this.Products.Remove(product);
            SoundService.PlaySuccess();
            ShowStatus($"Товар «{product.Name}» удалён", Color.FromArgb("#C42B1C"));
        }
    }

    // Поиск по названию товара и категории (ЛР7)
    private async void SearchButton_Clicked(object? sender, EventArgs e)
    {
        // Анимация нажатия кнопки
        if (sender is Button button)
        {
            await button.ScaleToAsync(0.96, 60);
            await button.ScaleToAsync(1.0, 60);
        }

        string? name = string.IsNullOrWhiteSpace(SearchName.Text) ? null : SearchName.Text.Trim();
        string? category = string.IsNullOrWhiteSpace(SearchCategory.Text) ? null : SearchCategory.Text.Trim();

        if (name is null && category is null)
        {
            LoadProducts();
            await DisplayAlertAsync("Ошибка", "Данные не были введены", "Ок");
            return;
        }

        var query = shopContext.Products.Include(p => p.Category).AsQueryable();

        if (name is not null)
        {
            query = query.Where(p => EF.Functions.Like(p.Name, $"%{name}%"));
        }

        if (category is not null)
        {
            query = query.Where(p => EF.Functions.Like(p.Category.Name, $"%{category}%"));
        }

        try
        {
            var found = query.ToList();
            this.collectionView.ItemsSource = new ObservableCollection<Product>(found);
            ShowStatus($"Найдено товаров: {found.Count}", Color.FromArgb("#0F6CBD"));
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Ошибка подключения к базе данных", ex.Message, "Ок");
        }
    }
}

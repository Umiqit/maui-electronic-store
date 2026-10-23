using System.Collections.ObjectModel;
using System.Windows.Input;
using MauiApp2_Lab2.Data;
using MauiApp2_Lab2.Models;
using Microsoft.EntityFrameworkCore;

namespace MauiApp2_Lab2;

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

    public HomePage()
    {
        InitializeComponent();

        shopContext = new ShopContext();
        Products = new ObservableCollection<Product>();

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

    // Загрузка данных при появлении страницы (ЛР4)
    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadProducts();
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
            await DisplayAlertAsync("Сохранить", "Изменения сохранены в базе данных", "Ок");
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
        }
    }

    // Поиск по названию товара и категории (ЛР7)
    private async void SearchButton_Clicked(object sender, EventArgs e)
    {
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

        var found = query.ToList();
        this.collectionView.ItemsSource = new ObservableCollection<Product>(found);
    }
}

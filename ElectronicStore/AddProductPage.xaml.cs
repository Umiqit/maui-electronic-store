using ElectronicStore.Data;
using ElectronicStore.Models;

namespace ElectronicStore;

public partial class AddProductPage : ContentPage
{
    private readonly ShopContext context;

    public AddProductPage()
    {
        InitializeComponent();
        context = new ShopContext();

        // Категории для выбора загружаются из базы данных
        try
        {
            CategoryPicker.ItemsSource = context.Categories.ToList();
        }
        catch (Exception ex)
        {
            _ = DisplayAlertAsync("Ошибка подключения к базе данных",
                $"Не удалось загрузить категории.\n\n{ex.Message}", "Ок");
        }
    }

    private async void AddProductButton_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameEntry.Text) ||
            !decimal.TryParse(PriceEntry.Text, out var price) ||
            !int.TryParse(StockEntry.Text, out var stock) ||
            CategoryPicker.SelectedItem is not Category category)
        {
            await DisplayAlertAsync("Ошибка", "Заполните все поля корректно", "Ок");
            return;
        }

        context.Products.Add(new Product
        {
            Name = NameEntry.Text.Trim(),
            Description = DescriptionEntry.Text,
            Price = price,
            StockQuantity = stock,
            CategoryId = category.Id
        });

        context.SaveChanges();
        await DisplayAlertAsync("Добавить", "Товар добавлен в базу данных", "Ок");
        await Navigation.PopAsync();
    }
}

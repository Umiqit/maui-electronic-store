using ElectronicStore.Data;
using ElectronicStore.Models;
using ElectronicStore.Services;

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
        // Анимация нажатия кнопки
        if (sender is Button button)
        {
            await button.ScaleTo(0.96, 60);
            await button.ScaleTo(1.0, 60);
        }

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

        try
        {
            context.SaveChanges();
            SoundService.PlaySuccess();
            await DisplayAlertAsync("Добавить", "Товар добавлен в базу данных", "Ок");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Ошибка", ex.Message, "Ок");
        }
    }
}

using ElectronicStore.Data;
using ElectronicStore.Models;
using ElectronicStore.Services;

namespace ElectronicStore;

public partial class EditProductPage : ContentPage
{
    private readonly ShopContext context;
    private readonly Product product;

    public EditProductPage(Product product)
    {
        InitializeComponent();
        this.product = product;
        context = new ShopContext();

        HeaderLabel.Text = $"Редактирование: {product.Name}";
        NameEntry.Text = product.Name;
        DescriptionEntry.Text = product.Description;
        PriceEntry.Text = product.Price.ToString("0.00");
        StockEntry.Text = product.StockQuantity.ToString();

        // Категории для выбора загружаются из базы данных
        try
        {
            var categories = context.Categories.ToList();
            CategoryPicker.ItemsSource = categories;
            CategoryPicker.SelectedItem = categories.FirstOrDefault(c => c.Id == product.CategoryId);
        }
        catch (Exception ex)
        {
            _ = DisplayAlertAsync("Ошибка подключения к базе данных",
                $"Не удалось загрузить категории.\n\n{ex.Message}", "Ок");
        }
    }

    private async void SaveButton_Clicked(object? sender, EventArgs e)
    {
        // Анимация нажатия кнопки
        if (sender is Button button)
        {
            await button.ScaleToAsync(0.96, 60);
            await button.ScaleToAsync(1.0, 60);
        }

        if (string.IsNullOrWhiteSpace(NameEntry.Text) ||
            !decimal.TryParse(PriceEntry.Text, out var price) ||
            !int.TryParse(StockEntry.Text, out var stock) ||
            CategoryPicker.SelectedItem is not Category category)
        {
            await DisplayAlertAsync("Ошибка", "Заполните все поля корректно", "Ок");
            return;
        }

        // ЛР5: изменение данных — обновляем поля выбранного товара
        product.Name = NameEntry.Text.Trim();
        product.Description = DescriptionEntry.Text;
        product.Price = price;
        product.StockQuantity = stock;
        product.CategoryId = category.Id;
        product.Category = category;

        context.Products.Update(product);

        try
        {
            context.SaveChanges();
            SoundService.PlaySuccess();
            await DisplayAlertAsync("Сохранить", "Изменения сохранены в базе данных", "Ок");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Ошибка", ex.Message, "Ок");
        }
    }
}

using ElectronicStore.Data;
using ElectronicStore.Models;
using ElectronicStore.Services;

namespace ElectronicStore;

public partial class AddCustomerPage : ContentPage
{
    private readonly ShopContext context;

    public AddCustomerPage()
    {
        InitializeComponent();
        context = new ShopContext();
    }

    private async void AddCustomerButton_Clicked(object? sender, EventArgs e)
    {
        // Анимация нажатия кнопки
        if (sender is Button button)
        {
            await button.ScaleToAsync(0.96, 60);
            await button.ScaleToAsync(1.0, 60);
        }

        if (string.IsNullOrWhiteSpace(NameEntry.Text) ||
            string.IsNullOrWhiteSpace(EmailEntry.Text))
        {
            await DisplayAlertAsync("Ошибка", "Заполните все поля корректно", "Ок");
            return;
        }

        context.Customers.Add(new Customer
        {
            Name = NameEntry.Text.Trim(),
            Email = EmailEntry.Text.Trim(),
            Phone = PhoneEntry.Text
        });

        try
        {
            context.SaveChanges();
            SoundService.PlaySuccess();
            await DisplayAlertAsync("Добавить", "Покупатель добавлен в базу данных", "Ок");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Ошибка", ex.Message, "Ок");
        }
    }
}

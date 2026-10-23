namespace MauiApp_Lab1;

public partial class HomePage : ContentPage
{
    public HomePage()
    {
        InitializeComponent();
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Отмена", "Действие отменено пользователем (ЛР1)", "Ок");
    }

    private async void OnCreateClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Создать", "Создание новой записи (ЛР1)", "Ок");
    }

    private async void OnEditClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Редактировать", "Редактирование записи (ЛР1)", "Ок");
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Сохранить", "Сохранение данных (ЛР1)", "Ок");
    }

    private async void OnFindClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Найти", "Поиск данных (ЛР1)", "Ок");
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Удалить", "Удаление записи (ЛР1)", "Ок");
    }

    private async void OnReportClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Отчёт", "Формирование отчёта (ЛР1)", "Ок");
    }
}

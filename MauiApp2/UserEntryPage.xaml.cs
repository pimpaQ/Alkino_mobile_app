using MauiApp1.Data;
using MauiApp1.Models;
using System.Collections.ObjectModel;

namespace MauiApp1;

public partial class UserEntryPage : ContentPage
{
    public ObservableCollection<Entry> UserEntries { get; set; }

    public UserEntryPage()
    {
        InitializeComponent();
        BindingContext = this;
        LoadUserData();
    }

    private async void GoBack_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void LoadUserData()
    {
        // Получаем текущего пользователя
        var userId = Session.CurrentUserId; // Идентификатор пользователя из сессии
        var currentUser = await DatabaseService.Database.Table<Users>()
                                  .FirstOrDefaultAsync(u => u.UserID == userId);

        // Устанавливаем имя пользователя

        // Получаем записи текущего пользователя, отсортированные по дате и времени
        var entries = await DatabaseService.Database.Table<Entry>()
                               .Where(e => e.UserID == userId)
                               .OrderByDescending(e => e.Date)
                               .ThenBy(e => e.Time)
                               .ToListAsync();

        foreach (var entry in entries)
        {
            // Загружаем связанные данные из ReasonList
            entry.Reason = await DatabaseService.Database.Table<ReasonList>()
                                      .FirstOrDefaultAsync(r => r.ReasonListId == entry.ReasonID);
        }

        // Устанавливаем ближайшую запись и остальные
        if (entries.Any())
        {
            // Устанавливаем данные для привязки
            UserEntries = new ObservableCollection<Entry>(entries);
            OnPropertyChanged(nameof(UserEntries));
        }
        else
        {
            // Если записей нет
            UserEntries = new ObservableCollection<Entry>();
            OnPropertyChanged(nameof(UserEntries));
        }
    }
}
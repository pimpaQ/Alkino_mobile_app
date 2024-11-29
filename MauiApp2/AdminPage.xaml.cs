using MauiApp1.Data;
using MauiApp1.Models;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MauiApp1;

public partial class AdminPage : ContentPage
{
    public ObservableCollection<EntryViewModel> Entries { get; set; } = new();
    public ObservableCollection<Entry> UserEntries { get; set; }
    public AdminPage()
	{
		InitializeComponent();
        BindingContext = this;
        LoadEntriesAsync();
    }
    private async void LoadEntriesAsync()
    {
        var entries = await DatabaseService.Database.Table<Entry>().ToListAsync();
        var users = await DatabaseService.Database.Table<Users>().ToListAsync();
        var reasons = await DatabaseService.Database.Table<ReasonList>().ToListAsync();

        // Соединяем данные
        foreach (var entry in entries)
        {
            var user = users.FirstOrDefault(u => u.UserID == entry.UserID);
            var reason = reasons.FirstOrDefault(r => r.ReasonListId == entry.ReasonID);

            if (user != null && reason != null)
            {
                Entries.Add(new EntryViewModel
                {
                    EntryId = entry.EntryID,
                    ServiceName = reason.ReasonName,
                    DateTime = $"{entry.Date} {entry.Time}",
                    UserName = $"{user.FirstName} {user.Name} {user.Patronymic}",
                    Description = entry.Description,
                    Access = entry.Acces
                });
            }
        }
    }

    private async void OnConfirmEntry_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var entry = button?.CommandParameter as EntryViewModel;

        if (entry == null)
            return;

        if (entry.Access == 1)
        {
            await DisplayAlert("Ошибка", "Запись уже подтверждена", "ОК");
            return;
        }

        var dbEntry = await DatabaseService.Database.Table<Entry>().FirstOrDefaultAsync(e => e.EntryID == entry.EntryId);
        if (dbEntry != null)
        {
            dbEntry.Acces = 1;
            await DatabaseService.Database.UpdateAsync(dbEntry);

            entry.Access = 1;
            await DisplayAlert("Успех", "Запись успешно подтверждена", "ОК");
        }
    }

    private async void ConfirmEntry(int entryId)
    {
        var entry = await DatabaseService.Database.Table<Entry>()
                                .FirstOrDefaultAsync(e => e.EntryID == entryId);
        if (entry != null && entry.Acces == 0)
        {
            entry.Acces = 1; // Обновляем статус
            await DatabaseService.Database.UpdateAsync(entry); // Сохраняем изменения

            // Обновляем данные в UI
            var updatedEntries = await DatabaseService.Database.Table<Entry>()
                                    .Where(e => e.UserID == Session.CurrentUserId)
                                    .OrderByDescending(e => e.Date)
                                    .ThenBy(e => e.Time)
                                    .ToListAsync();
            UserEntries = new ObservableCollection<Entry>(updatedEntries);
            OnPropertyChanged(nameof(UserEntries));
        }
    }

}
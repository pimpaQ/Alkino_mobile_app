using MauiApp1.Data;
using MauiApp1.Models;
using System.Collections.ObjectModel;

namespace MauiApp1;

public partial class UserEntryPage : ContentPage
{
    public ObservableCollection<Entry> AllEntries { get; set; }
    public ObservableCollection<Entry> FilteredEntries { get; set; }

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
        var userId = Session.CurrentUserId;
        var entries = await DatabaseService.Database.Table<Entry>()
                           .Where(e => e.UserID == userId)
                           .ToListAsync();

        foreach (var entry in entries)
        {
            entry.Reason = await DatabaseService.Database.Table<ReasonList>()
                                  .FirstOrDefaultAsync(r => r.ReasonListId == entry.ReasonID);
        }
        AllEntries = new ObservableCollection<Entry>(entries);
        FilteredEntries = new ObservableCollection<Entry>(entries.Where(e => e.Acces == 1));
        OnPropertyChanged(nameof(FilteredEntries));
    }
    private string _selectedGroup;

    private void HighlightButton(string selectedGroup)
    {
        // Сбросить фон всех кнопок
        confirmBtn.BackgroundColor = Colors.Transparent;
        waitBtn.BackgroundColor = Colors.Transparent;
        rejectBtn.BackgroundColor = Colors.Transparent;

        // Подсветить выбранную кнопку
        switch (selectedGroup)
        {
            case "Confirmed":
                confirmBtn.BackgroundColor = Color.FromArgb("#D0FFD6"); // Зеленоватый
                break;
            case "Waiting":
                waitBtn.BackgroundColor = Color.FromArgb("#FFF2D6"); // Желтоватый
                break;
            case "Rejected":
                rejectBtn.BackgroundColor = Color.FromArgb("#FFD6D6"); // Красноватый
                break;
        }

        _selectedGroup = selectedGroup; // Обновляем текущую группу
    }

    private void confirmBtn_Clicked(object sender, EventArgs e)
    {
        HighlightButton("Confirmed");
        FilteredEntries = new ObservableCollection<Entry>(AllEntries.Where(e => e.Acces == 1));
        OnPropertyChanged(nameof(FilteredEntries));
    }

    private void waitBtn_Clicked(object sender, EventArgs e)
    {
        HighlightButton("Waiting");
        FilteredEntries = new ObservableCollection<Entry>(AllEntries.Where(e => e.Acces == 0));
        OnPropertyChanged(nameof(FilteredEntries));
    }

    private void rejectBtn_Clicked(object sender, EventArgs e)
    {
        HighlightButton("Rejected");
        FilteredEntries = new ObservableCollection<Entry>(AllEntries.Where(e => e.Acces == 2));
        OnPropertyChanged(nameof(FilteredEntries));
    }
}
using MauiApp1.Data;
using MauiApp1.Models;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MauiApp1;

public partial class AdminPage : ContentPage
{
    public ObservableCollection<EntryViewModel> ConfirmedEntries { get; set; } = new();
    public ObservableCollection<EntryViewModel> PendingEntries { get; set; } = new();
    public ObservableCollection<EntryViewModel> RejectedEntries { get; set; } = new();
    public ObservableCollection<EntryViewModel> CurrentEntries { get; set; } = new();

    public AdminPage()
	{
		InitializeComponent();
        BindingContext = this;
        LoadEntriesAsync();
    }
    private void UpdateCurrentEntries(ObservableCollection<EntryViewModel> source)
    {
        CurrentEntries.Clear();
        foreach (var entry in source)
        {
            CurrentEntries.Add(entry);
        }

        OnPropertyChanged(nameof(CurrentEntries));
    }
    private async void LoadEntriesAsync()
    {
        var entries = await DatabaseService.Database.Table<Entry>().ToListAsync();
        var users = await DatabaseService.Database.Table<Users>().ToListAsync();
        var reasons = await DatabaseService.Database.Table<ReasonList>().ToListAsync();

        ConfirmedEntries.Clear();
        PendingEntries.Clear();
        RejectedEntries.Clear();

        foreach (var entry in entries)
        {
            var user = users.FirstOrDefault(u => u.UserID == entry.UserID);
            var reason = reasons.FirstOrDefault(r => r.ReasonListId == entry.ReasonID);

            if (user != null && reason != null)
            {
                var entryViewModel = new EntryViewModel
                {
                    EntryId = entry.EntryID,
                    ServiceName = reason.ReasonName,
                    DateTime = $"{entry.Date} {entry.Time}",
                    UserName = $"{user.FirstName} {user.Name} {user.Patronymic}",
                    Description = entry.Description,
                    Access = entry.Acces
                };

                switch (entry.Acces)
                {
                    case 1:
                        ConfirmedEntries.Add(entryViewModel);
                        break;
                    case 0:
                        PendingEntries.Add(entryViewModel);
                        break;
                    case 2:
                        RejectedEntries.Add(entryViewModel);
                        break;
                }
            }
        }
        // По умолчанию показываем подтвержденные записи
        UpdateCurrentEntries(ConfirmedEntries);
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


    private void confirmBtn_Clicked(object sender, EventArgs e)
    {
        UpdateCurrentEntries(ConfirmedEntries);
        labelll.Text = "Подтвержденные записи";
    }

    private void waitToconfirmBtn_Clicked(object sender, EventArgs e)
    {
        UpdateCurrentEntries(PendingEntries);
        labelll.Text = "Ожидающие подтверждение записи";
    }

    private void rejectBtn_Clicked(object sender, EventArgs e)
    {
        UpdateCurrentEntries(RejectedEntries);
        labelll.Text = "Отклоненные записи";
    }

    private async void OnConfirmBtn_Clicked(object sender, EventArgs e)
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
            dbEntry.Acces = 1; // Устанавливаем статус "подтверждена"
            await DatabaseService.Database.UpdateAsync(dbEntry);

            // Обновляем UI
            entry.Access = 1;
            PendingEntries.Remove(entry);
            ConfirmedEntries.Add(entry);
            UpdateCurrentEntries(CurrentEntries);

            await DisplayAlert("Успех", "Запись успешно подтверждена", "ОК");
        }
    }

    private async void OnRejectBtn_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var entry = button?.CommandParameter as EntryViewModel;

        if (entry == null)
            return;

        // Ввод причины отказа через DisplayPromptAsync
        string reason = await DisplayPromptAsync("Причина отказа", "Введите причину отклонения записи:",
                                                 "Отклонить", "Отмена",
                                                 placeholder: "Причина отказа", maxLength: 200);

        if (string.IsNullOrWhiteSpace(reason))
        {
            await DisplayAlert("Ошибка", "Необходимо указать причину отказа.", "ОК");
            return;
        }

        // Обновляем запись в БД с причиной отказа
        var dbEntry = await DatabaseService.Database.Table<Entry>().FirstOrDefaultAsync(e => e.EntryID == entry.EntryId);
        if (dbEntry != null)
        {
            dbEntry.Acces = 2; // Отклоненная запись
            dbEntry.RejectDescription = $"Причина отказа: {reason}"; // Сохраняем причину в описании
            await DatabaseService.Database.UpdateAsync(dbEntry);

            // Обновляем локальные данные
            entry.Access = 2;
            entry.Description = $"Отклонено: {reason}";

            RejectedEntries.Add(entry); // Добавляем запись в отклоненные
            PendingEntries.Remove(entry); // Убираем из ожидающих

            UpdateCurrentEntries(RejectedEntries); // Обновляем текущий список

            await DisplayAlert("Успех", "Запись успешно отклонена.", "ОК");
        }
    }
}
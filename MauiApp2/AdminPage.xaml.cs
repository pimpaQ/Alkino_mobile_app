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
        // �� ��������� ���������� �������������� ������
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
            await DisplayAlert("������", "������ ��� ������������", "��");
            return;
        }

        var dbEntry = await DatabaseService.Database.Table<Entry>().FirstOrDefaultAsync(e => e.EntryID == entry.EntryId);
        if (dbEntry != null)
        {
            dbEntry.Acces = 1;
            await DatabaseService.Database.UpdateAsync(dbEntry);

            entry.Access = 1;
            await DisplayAlert("�����", "������ ������� ������������", "��");
        }
    }


    private void confirmBtn_Clicked(object sender, EventArgs e)
    {
        UpdateCurrentEntries(ConfirmedEntries);
        labelll.Text = "�������������� ������";
    }

    private void waitToconfirmBtn_Clicked(object sender, EventArgs e)
    {
        UpdateCurrentEntries(PendingEntries);
        labelll.Text = "��������� ������������� ������";
    }

    private void rejectBtn_Clicked(object sender, EventArgs e)
    {
        UpdateCurrentEntries(RejectedEntries);
        labelll.Text = "����������� ������";
    }

    private async void OnConfirmBtn_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var entry = button?.CommandParameter as EntryViewModel;

        if (entry == null)
            return;

        if (entry.Access == 1)
        {
            await DisplayAlert("������", "������ ��� ������������", "��");
            return;
        }

        var dbEntry = await DatabaseService.Database.Table<Entry>().FirstOrDefaultAsync(e => e.EntryID == entry.EntryId);
        if (dbEntry != null)
        {
            dbEntry.Acces = 1; // ������������� ������ "������������"
            await DatabaseService.Database.UpdateAsync(dbEntry);

            // ��������� UI
            entry.Access = 1;
            PendingEntries.Remove(entry);
            ConfirmedEntries.Add(entry);
            UpdateCurrentEntries(CurrentEntries);

            await DisplayAlert("�����", "������ ������� ������������", "��");
        }
    }

    private async void OnRejectBtn_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var entry = button?.CommandParameter as EntryViewModel;

        if (entry == null)
            return;

        // ���� ������� ������ ����� DisplayPromptAsync
        string reason = await DisplayPromptAsync("������� ������", "������� ������� ���������� ������:",
                                                 "���������", "������",
                                                 placeholder: "������� ������", maxLength: 200);

        if (string.IsNullOrWhiteSpace(reason))
        {
            await DisplayAlert("������", "���������� ������� ������� ������.", "��");
            return;
        }

        // ��������� ������ � �� � �������� ������
        var dbEntry = await DatabaseService.Database.Table<Entry>().FirstOrDefaultAsync(e => e.EntryID == entry.EntryId);
        if (dbEntry != null)
        {
            dbEntry.Acces = 2; // ����������� ������
            dbEntry.RejectDescription = $"������� ������: {reason}"; // ��������� ������� � ��������
            await DatabaseService.Database.UpdateAsync(dbEntry);

            // ��������� ��������� ������
            entry.Access = 2;
            entry.Description = $"���������: {reason}";

            RejectedEntries.Add(entry); // ��������� ������ � �����������
            PendingEntries.Remove(entry); // ������� �� ���������

            UpdateCurrentEntries(RejectedEntries); // ��������� ������� ������

            await DisplayAlert("�����", "������ ������� ���������.", "��");
        }
    }
}
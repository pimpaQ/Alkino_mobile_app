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
        // �������� �������� ������������
        var userId = Session.CurrentUserId; // ������������� ������������ �� ������
        var currentUser = await DatabaseService.Database.Table<Users>()
                                  .FirstOrDefaultAsync(u => u.UserID == userId);

        // ������������� ��� ������������

        // �������� ������ �������� ������������, ��������������� �� ���� � �������
        var entries = await DatabaseService.Database.Table<Entry>()
                               .Where(e => e.UserID == userId)
                               .OrderByDescending(e => e.Date)
                               .ThenBy(e => e.Time)
                               .ToListAsync();

        foreach (var entry in entries)
        {
            // ��������� ��������� ������ �� ReasonList
            entry.Reason = await DatabaseService.Database.Table<ReasonList>()
                                      .FirstOrDefaultAsync(r => r.ReasonListId == entry.ReasonID);
        }

        // ������������� ��������� ������ � ���������
        if (entries.Any())
        {
            // ������������� ������ ��� ��������
            UserEntries = new ObservableCollection<Entry>(entries);
            OnPropertyChanged(nameof(UserEntries));
        }
        else
        {
            // ���� ������� ���
            UserEntries = new ObservableCollection<Entry>();
            OnPropertyChanged(nameof(UserEntries));
        }
    }
}
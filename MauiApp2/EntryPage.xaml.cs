using MauiApp1.Data;
using MauiApp1.Resources.Strings;

namespace MauiApp1;

public partial class EntryPage : ContentPage
{
    private static readonly List<string> AllTimeSlots = new()
    {
        "09:30", "10:00", "10:30", "11:00", "11:30",
        "12:00", "12:30", "14:00", "14:30", "15:00", "15:30", "16:00", "16:30"
    };
   
    public EntryPage()
	{
		InitializeComponent();
        DateSelector.DateSelected += DateSelector_DateSelected;

        var options = new List<string>
        {
            AppResources.r1,
            AppResources.r2,
            AppResources.r3,
            AppResources.r4,
            AppResources.r5,
            AppResources.r6,
            AppResources.r7,
            AppResources.r8
        };
        picker.ItemsSource = options;
    }
    private async void DateSelector_DateSelected(object sender, DateChangedEventArgs e)
    {
        string selectedDate = DateSelector.Date.ToString("yyyy-MM-dd");
        var entries = await DatabaseService.Database.Table<Entry>()
                          .Where(x => x.Date == selectedDate)
                          .ToListAsync();

        var takenSlots = entries.Select(x => x.Time).ToList();
        var availableSlots = AllTimeSlots.Except(takenSlots).ToList();

        TimeSelector.ItemsSource = availableSlots;
    }
    private async void EntryBtn_Clicked(object sender, EventArgs e)
    {
        if (picker.SelectedItem is null)
        {
            await DisplayAlert(AppResources.errorMsg, AppResources.reasonChoose, "问");
            return;
        }
        if (DateSelector is null)
        {
            await DisplayAlert(AppResources.errorMsg, AppResources.dateChoose, "问");
            return;
        }
        if (TimeSelector.SelectedItem is null)
        {
            await DisplayAlert(AppResources.errorMsg, AppResources.chooseTime, "问");
            return;
        }

        var entry = new Entry()
        {
            ReasonID = picker.SelectedIndex + 1,
            UserID = Session.CurrentUserId,
            Date = DateSelector.Date.ToString("yyyy-MM-dd"),
            Time = TimeSelector.SelectedItem.ToString(),
            Description = DescriptionEditor.Text
        };
        await DatabaseService.Database.InsertAsync(entry);
        await DisplayAlert(AppResources.successMsg, AppResources.prMsg, "问");
        await Navigation.PushAsync(new NewPage1());
    }

    private async void mainpageBtn_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new NewPage1());
    }

    private async void profilepageBtn_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ProfilePage());
    }

    private async void newsBtn_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new NewsPage());
    }
}



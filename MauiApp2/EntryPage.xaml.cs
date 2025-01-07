using MauiApp1.Data;

namespace MauiApp1;

public partial class EntryPage : ContentPage
{
	public EntryPage()
	{
		InitializeComponent();
	}

    private async void EntryBtn_Clicked(object sender, EventArgs e)
    {
        var entry = new Entry()
        {
            ReasonID = picker.SelectedIndex + 1,
            UserID = Session.CurrentUserId,
            Date = DateSelector.Date.ToString("yyyy-MM-dd"),
            Time = TimeSelector.Time.ToString(@"hh\:mm"),
            Description = DescriptionEditor.Text
        };
        await DatabaseService.Database.InsertAsync(entry);
        await DisplayAlert("Успех", "Вы записались на приём!", "ОК");
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



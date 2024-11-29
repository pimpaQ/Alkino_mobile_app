namespace MauiApp1;

public partial class NavigationBar : ContentView
{
	public NavigationBar()
	{
		InitializeComponent();
        ToEntryPageButton.Clicked += ToEntryPageButton_Clicked;
    }
    private async void ToEntryPageButton_Clicked(object sender, EventArgs e)
    {
        await Application.Current.MainPage.Navigation.PushAsync(new EntryPage());
    }

    private void profileBtn_Clicked(object sender, EventArgs e)
    {

    }

    private void mainpageBtn_Clicked(object sender, EventArgs e)
    {

    }
}
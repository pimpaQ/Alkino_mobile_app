﻿using MauiApp1.Data;
using MauiApp1.Models;
using System.Globalization;

namespace MauiApp1;

public partial class ProfilePage : ContentPage
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }

    public ProfilePage()
    {
        InitializeComponent();
        BindingContext = this;
        if(langlab.Text == "Выберите язык")
            LangPicker.SelectedIndex = 0;
        else
            LangPicker.SelectedIndex = 1;
        LoadUserData();
    }

    private async void LoadUserData()
    {
        var userId = Session.CurrentUserId; // Получаем ID текущего пользователя
        var user = await DatabaseService.Database.Table<Users>()
                        .FirstOrDefaultAsync(u => u.UserID == userId);

        if (user != null)
        {
            FullName = $"{user.FirstName} {user.Name} {user.Patronymic}";
            Email = user.Email;
            PhoneNumber = user.Phone;
            OnPropertyChanged(nameof(FullName));
            OnPropertyChanged(nameof(Email));
            OnPropertyChanged(nameof(PhoneNumber));
        }
    }

    private async void mainPageBtn_Clicked(object sender, EventArgs e)
    {
        Navigation.InsertPageBefore(new NewPage1(), this);
        await Navigation.PopAsync();
    }

    private async void EntryPageBtn_Clicked(object sender, EventArgs e)
    {
        Navigation.InsertPageBefore(new EntryPage(), this);
        await Navigation.PopAsync();
    }

    private async void SaveBtn_Clicked(object sender, EventArgs e)
    {
        var userId = Session.CurrentUserId;
        var user = await DatabaseService.Database.Table<Users>()
                        .FirstOrDefaultAsync(u => u.UserID == userId);
        string langCode;
        if (user != null)
        {
            if (LangPicker.SelectedIndex == 1)
            {
                langCode = "ba";
                LangPicker.SelectedIndex = 1;
            }
            else
            {
                langCode = "ru";
                LangPicker.SelectedIndex = 0;
            }
            Preferences.Set("AppLanguage", langCode);
            CultureInfo culture = new CultureInfo(langCode);
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            user.Email = Email; // Обновляем почту
            await DatabaseService.Database.UpdateAsync(user); // Сохраняем изменения
            await DisplayAlert("Успех", "Данные сохранены.", "ОК");
            Navigation.InsertPageBefore(new NewPage1(), this);
            await Navigation.PopAsync();
        }
        else
        {
            await DisplayAlert("Ошибка", "Не удалось найти пользователя.", "ОК");
        }
    }

    private async void newsBtn_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new NewsPage());
    }
}
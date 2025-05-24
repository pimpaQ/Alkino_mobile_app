using MauiApp1.Models;
using MauiApp1.Resources.Strings;
using SQLite;

public class Entry
{
    [PrimaryKey, AutoIncrement]
    public int EntryID { get; set; }
    public int UserID { get; set; }
    public int ReasonID { get; set; }
    public string Date { get; set; }
    public string Time { get; set; }
    public int Acces { get; set; }
    public string Description { get; set; }
    public string RejectDescription { get; set; }

    [Ignore]
    public Users User { get; set; }

    [Ignore] 
    public ReasonList Reason { get; set; }

    public string Status
    {
        get
        {
            if(Acces == 1)
            {
                return AppResources.s1;
            }
            else if(Acces == 0)
            {
                return AppResources.s2;
            }
            else
            {
                return AppResources.s3;
            }
        }
    }
    public static string GetLocalizedReason(string reasonFromDb)
    {
        return reasonFromDb switch
        {
            "Переустройство/перепланировка помещения в многоквартирном доме" => AppResources.r1,
            "Принять на учет гражданина, нуждающимся в жил.помещениях" => AppResources.r2,
            "Бытовая характеристика гражданина" => AppResources.r3,
            "Справка о составе семьи" => AppResources.r4,
            "Выписка из ЕГРН" => AppResources.r5,
            "Справка с места жительства умершего" => AppResources.r6,
            "Разрешение на захоронение" => AppResources.r7,
            _ => reasonFromDb
        };
    }
    public string ReasonLocalized
    {
        get
        {
            return Reason != null
                ? GetLocalizedReason(Reason.ReasonName)
                : string.Empty;
        }
    }
}
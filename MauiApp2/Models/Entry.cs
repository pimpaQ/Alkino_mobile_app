using MauiApp1.Models;
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
                return "Подтверждена";
            }
            else if(Acces == 0)
            {
                return "Ожидает подтверждения";
            }
            else
            {
                return "Отклонена";
            }
        }
    }

}
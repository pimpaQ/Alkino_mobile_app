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

    [Ignore]
    public Users User { get; set; }

    [Ignore] 
    public ReasonList Reason { get; set; }

    public string Status => Acces == 1 ? "Подтверждена" : "Ожидает подтверждения";
}
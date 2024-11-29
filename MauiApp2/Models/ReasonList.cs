using SQLite;
using MauiApp1.Models;
using System.ComponentModel.DataAnnotations;

namespace MauiApp1.Models
{
    public class ReasonList
    {
        [PrimaryKey, AutoIncrement]
        public int ReasonListId { get; set; }
        public string ReasonName { get; set; }
        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Models
{
    public class EntryViewModel
    {
        public int EntryId { get; set; }
        public string ServiceName { get; set; } // Название услуги
        public string DateTime { get; set; }   // Дата и время
        public string UserName { get; set; }   // ФИО пользователя
        public string Description { get; set; } // Примечание
        public string RejectDescription { get; set; }
        public int Access { get; set; }
    }

}
